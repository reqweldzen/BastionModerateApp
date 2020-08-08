using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BastionModerateApp.Entities;
using BastionModerateApp.Enums;
using BastionModerateApp.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BastionModerateApp.Modules
{
	[Group("invite")]
	public class InviteModule : ModuleBase
	{
		private readonly BastionContext _db;

		public InviteModule(DiscordSocketClient client, BastionContext db)
		{
			_db = db;
		}

		private Embed CreateEmbed(ContentTemplate template, PartyInvite invite)
		{
			var contentName = !string.IsNullOrEmpty(template.QuestUrl)
				? $"[{template.ContentName}]({template.QuestUrl})"
				: template.ContentName;

			return new EmbedBuilder
				{
					Title = "パーティ募集",
					Description = $"#{invite.PartyInviteId}"
				}
				.AddField(builder =>
				{
					builder.Name = "高難易度コンテンツ";
					builder.Value = contentName;
				})
				.AddField("目的", invite.Purpose.DisplayName(), true)
				.AddField("開始日時", invite.StartDate.ToString("yyyy/MM/dd HH:mm"), true)
				.AddField("参加人数", $"0/8 人", true)
				.WithAuthor(Context.Message.Author)
				.WithColor(Color.Purple)
				.WithCurrentTimestamp()
				.Build();
		}

		[Command("create")]
		public async Task Invite(string? shortcutName = null, int purpose = 1, DateTime? startDate = null,
			DateTime? endDate = null)
		{
			var template = _db.ContentTemplates.FirstOrDefault(x => x.ShortcutName == shortcutName);
			if (template == null) return;

			var user = await _db.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id);
			if (user == null)
			{
				await ReplyAsync("You are not registered.");
				return;
			}

			var invite = new PartyInvite
			{
				UserId = user.UserId,
				ContentTemplateId = template.ContentTemplateId,
				Purpose = (Purpose) purpose,
				StartDate = startDate ?? DateTime.Now,
				EndDate = endDate
			};

			await _db.PartyInvites.AddAsync(invite);
			await _db.SaveChangesAsync();

			var embed = CreateEmbed(template, invite);

			var inviteMessage = await ReplyAsync(embed: embed);

			invite.MessageId = inviteMessage.Id;
			_db.PartyInvites.Update(invite);
			await _db.SaveChangesAsync();
		}

		[Command("finish")]
		public async Task InviteFinish(int id)
		{
			var invite = await _db.PartyInvites.FindAsync(id);
			if (invite == null) return;

			var message = await Context.Channel.GetMessageAsync(invite.MessageId) as IUserMessage;
			if (message?.Embeds.Any() != true) return;

			var inviter = await Context.Channel.GetUserAsync(invite.User.DiscordId);
			if (inviter == null) return;

			invite.IsFinished = true;
			_db.PartyInvites.Update(invite);
			await _db.SaveChangesAsync();

			await message.ModifyAsync(properties =>
			{
				properties.Embed = CreateFinishEmbed(message, invite, inviter).Build();
			});
		}

		[Command("show")]
		public async Task ShowInvitedMember(int id)
		{
			var invite = await _db.PartyInvites.FindAsync(id);
			if (invite == null) return;

			var jobs = _db.Jobs.ToList();
			var emotes = Context.Guild.Emotes.Where(x => jobs.Select(y => y.ShortcutName).Contains(x.Name)).ToList();

			var party = invite.PartyInviteEntries
				.Join(
					jobs,
					t => t.JobId,
					t => t.JobId,
					(p, j) => new {p, j}
				)
				.Join(
					_db.Users.ToList(),
					t => t.p.User.DiscordId,
					t => t.DiscordId,
					(t, u) => new {t.p, t.j, u})
				.ToList();

			var builder = new StringBuilder();
			builder.AppendLine($"#{invite.PartyInviteId} {invite.ContentTemplate.ContentName}");
			foreach (var (entry, job, user) in party.Select(x => (x.p, x.j, x.u)))
			{
				builder.AppendLine($"{emotes.FirstOrDefault(x => x.Name == job.ShortcutName)} {user.PlayerName}");
			}

			await ReplyAsync(builder.ToString());
		}

		[Command("update")]
		public async Task UpdateInviteData(int id)
		{
			var invite = await _db.PartyInvites.FindAsync(id);
			if (invite == null) return;

			var inviter = invite.User;
			
			var message = await Context.Channel.GetMessageAsync(invite.MessageId) as IUserMessage;
			if (message?.Embeds.Any() != true)
			{
				await ReplyAsync($"The invitation message was not found.");
				return;
			}

			var jobs = _db.Jobs.ToList();
			var emotes = GetGuildEmotes(jobs);

			await using (var transaction = await _db.Database.BeginTransactionAsync())
			{
				try
				{
					var entries = invite.PartyInviteEntries.ToList();
					_db.PartyInviteEntries.RemoveRange(entries);
					await _db.SaveChangesAsync();
					
					var emoteUsers = new List<EmoteUser>();
					foreach (var emote in emotes)
					{
						await foreach (var user in message.GetReactionUsersAsync(emote, 20))
						{
							emoteUsers.AddRange(user.Select(x => new EmoteUser {User = x, Emote = emote,}));
						}
					}

					var entities = emoteUsers.Join(
						jobs,
						t => t.Emote.Name,
						t => t.ShortcutName,
						(emoteUser, job) => new PartyInviteEntry
						{
							PartyInviteId = invite.PartyInviteId,
							UserId = inviter.UserId,
							JobId = job.JobId,
						});

					await _db.AddRangeAsync(entities);
					
					await _db.SaveChangesAsync();

					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					await ReplyAsync(e.Message);
					return;
				}
			}

			await ReplyAsync($"#{invite.PartyInviteId} synced!");
		}

		private IEnumerable<GuildEmote> GetGuildEmotes(IEnumerable<Job> jobs) => Context.Guild.Emotes
			.Where(x => jobs.Select(y => y.ShortcutName).Contains(x.Name)).ToList();

		// [Command("invite-r")]
		// public async Task RefreshInvite(int id)
		// {
		// 	var invite = await _db.PartyInvites.FindAsync(id);
		// 	if (invite == null) return;
		//
		// 	var message = await Context.Channel.GetMessageAsync(invite.MessageId) as IUserMessage;
		// 	if (message?.Embeds.Any() != true) return;
		//
		// 	var jobs = _db.CharacterJobs.ToList();
		// 	var emotes = Context.Guild.Emotes.Where(x => jobs.Select(y => y.ShortcutName).Contains(x.Name)).ToList();
		//
		// 	var entries = invite.PartyInviteEntries.ToList();
		//
		// 	var users = new List<(IUser user, GuildEmote emote)>();
		// 	foreach (var emote in emotes)
		// 	{
		// 		await foreach (var user in message.GetReactionUsersAsync(emote, 20))
		// 		{
		// 			users.AddRange(user.Select(x => (x, emote)));
		// 		}
		// 	}
		//
		// 	using (var transaction = await _db.Database.BeginTransactionAsync())
		// 	{
		// 		try
		// 		{
		// 			foreach (var entry in entries)
		// 			{
		// 				if (!users.Any(x => x.user.Id == entry.User.DiscordId))
		// 				{
		// 					_db.Remove(entry);
		// 					await _db.SaveChangesAsync();
		// 					continue;
		// 				}
		//
		// 				var jobUser = users.FirstOrDefault(x => x.user.Id == entry.User.DiscordId);
		// 				var job = jobs.First(x => x.ShortcutName == jobUser.emote.Name);
		// 				if (entry.JobId != job.JobId)
		// 				{
		// 					entry.JobId = job.JobId;
		// 					// entry.ReactionName = jobUser.emote.Name;
		//
		// 					_db.Update(entry);
		// 					await _db.SaveChangesAsync();
		// 					continue;
		// 				}
		//
		// 				var entity = new PartyInviteEntry
		// 				{
		// 					PartyInviteId = invite.PartyInviteId,
		// 					// ReactionName = jobUser.emote.Name,
		// 					// UserId = jobUser.user.Id,
		// 					// CharacterJobId = job.JobId
		// 				};
		//
		// 				await _db.PartyInviteEntries.AddAsync(entry);
		// 				await _db.SaveChangesAsync();
		// 			}
		//
		// 			await transaction.CommitAsync();
		// 		}
		// 		catch (Exception)
		// 		{
		// 			transaction.Rollback();
		// 			return;
		// 		}
		// 	}
		//
		// 	var inviter = await Context.Channel.GetUserAsync(1);
		//
		// 	await message.ModifyAsync(properties => { properties.Embed = CreateEmbed(message, invite, inviter); });
		// }

		private Embed CreateEmbed(IUserMessage message, PartyInvite invite, IUser user)
		{
			var template = invite.ContentTemplate;
			var contentName = !string.IsNullOrEmpty(template.QuestUrl)
				? $"[{template.ContentName}]({template.QuestUrl})"
				: template.ContentName;

			return new EmbedBuilder
				{
					Title = "パーティ募集",
					Description = $"#{invite.PartyInviteId}"
				}
				.AddField(builder =>
				{
					builder.Name = "高難易度コンテンツ";
					builder.Value = contentName;
				})
				.AddField("目的", invite.Purpose.DisplayName(), true)
				.AddField("開始日時", invite.StartDate.ToString("yyyy/MM/dd HH:mm"), true)
				.AddField("参加人数", $"{invite.PartyInviteEntries.Count}/8 人", true)
				.WithAuthor(user)
				.WithColor(Color.Purple)
				.WithCurrentTimestamp()
				.Build();
		}

		private EmbedBuilder CreateFinishEmbed(IUserMessage message, PartyInvite invite, IUser user)
		{
			var template = invite.ContentTemplate;
			var contentName = !string.IsNullOrEmpty(template.QuestUrl)
				? $"[{template.ContentName}]({template.QuestUrl})"
				: template.ContentName;

			return new EmbedBuilder
				{
					Title = "パーティ募集 (終了)",
					Description = $"#{invite.PartyInviteId}"
				}
				.AddField(builder =>
				{
					builder.Name = "高難易度コンテンツ";
					builder.Value = contentName;
				})
				.AddField("目的", invite.Purpose.DisplayName(), true)
				.AddField("開始日時", invite.StartDate.ToString("yyyy/MM/dd HH:mm"), true)
				.WithAuthor(user)
				.WithColor(Color.Purple)
				.WithCurrentTimestamp();
		}
	}
}