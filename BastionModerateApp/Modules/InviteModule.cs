using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BastionModerateApp.Entities;
using BastionModerateApp.Enums;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BastionModerateApp.Modules
{
	public class InviteModule : ModuleBase
	{
		private readonly BastionContext _db;

		public InviteModule(DiscordSocketClient client, BastionContext db)
		{
			_db = db;
		}

		private Embed CreateEmbed(RaidTemplate template, PartyInvite invite)
		{
			var contentName = !string.IsNullOrEmpty(template.QuestUrl)
				? $"[{template.RaidName}]({template.QuestUrl})"
				: template.RaidName;

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

		[Command("invite")]
		public async Task Invite(string shortcutName = null, int purpose = 1, DateTime? startDate = null)
		{
			var template = _db.RaidTemplates.FirstOrDefault(x => x.ShortcutName == shortcutName);
			if (template == null)
			{
				return;
			}

			var invite = new PartyInvite
			{
				UserId = Context.User.Id,
				RaidTemplateId = template.RaidTemplateId,
				Purpose = (Purpose) purpose,
				StartDate = startDate ?? DateTime.Now
			};

			await _db.PartyInvites.AddAsync(invite);
			await _db.SaveChangesAsync();

			var embed = CreateEmbed(template, invite);

			var inviteMessage = await ReplyAsync(embed: embed);

			invite.MessageId = inviteMessage.Id;
			_db.PartyInvites.Update(invite);
			await _db.SaveChangesAsync();
		}

		[Command("invite-f")]
		public async Task InviteFinish(int id)
		{
			var invite = await _db.PartyInvites.FindAsync(id);
			if (invite == null) return;
			
			var message = await Context.Channel.GetMessageAsync(invite.MessageId) as IUserMessage;
			if (message?.Embeds.Any() != true) return;

			var inviter = await Context.Channel.GetUserAsync(invite.UserId);
			if (inviter == null) return;
			
			invite.IsFinished = true;
			_db.PartyInvites.Update(invite);
			await _db.SaveChangesAsync();
			
			

			await message.ModifyAsync(properties =>
			{
				properties.Embed = CreateFinishEmbed(message, invite, inviter).Build();
			});
		}

		[Command("invite-s")]
		public async Task ShowInvitedMember(int id)
		{
			var invite = await _db.PartyInvites.FindAsync(id);
			if (invite == null) return;

			var jobs = _db.CharacterJobs.ToList();
			var emotes = Context.Guild.Emotes.Where(x => jobs.Select(y => y.ReactionName).Contains(x.Name)).ToList();

			var party = invite.PartyInviteEntries
				.Join(
					jobs,
					t => t.CharacterJobId,
					t => t.JobId,
					(p, j) => new {p, j}
				)
				.Join(
					_db.Users.ToList(),
					t =>t.p.UserId,
					t => t.DiscordId,
					(t, u) => new {t.p, t.j, u})
				.ToList();
			
			var builder = new StringBuilder();
			builder.AppendLine($"#{invite.PartyInviteId} {invite.RaidTemplate.RaidName}");
			foreach (var (entry, job, user) in party.Select(x => (x.p, x.j, x.u)))
			{
				builder.AppendLine($"{emotes.FirstOrDefault(x => x.Name == job.ReactionName)} {user.PlayerName}");
			}

			await ReplyAsync(builder.ToString());
		}
		
		[Command("invite-d")]
		public async Task SyncDatabaseInvite(int id)
		{
			var invite = await _db.PartyInvites.FindAsync(id);
			if (invite == null) return;

			var message = await Context.Channel.GetMessageAsync(invite.MessageId) as IUserMessage;
			if (message?.Embeds.Any() != true) return;
			
			var jobs = _db.CharacterJobs.ToList();
			var emotes = Context.Guild.Emotes.Where(x => jobs.Select(y => y.ReactionName).Contains(x.Name)).ToList();

			using (var transaction = _db.Database.BeginTransaction())
			{
				try
				{
					var entries = invite.PartyInviteEntries.ToList();
					_db.PartyInviteEntries.RemoveRange(entries);
					await _db.SaveChangesAsync();

					var users = new List<(IUser user, GuildEmote emote)>();
					foreach (var emote in emotes)
					{
						await foreach (var user in message.GetReactionUsersAsync(emote, 20))
						{
							users.AddRange(user.Select(x => (x, emote)));
						}
					}

					foreach (var (user, emote) in users)
					{
						var job = jobs.First(x => x.ReactionName == emote.Name);

						var entity = new PartyInviteEntry
						{
							PartyInviteId = invite.PartyInviteId,
							ReactionName = emote.Name,
							UserId = user.Id,
							CharacterJobId = job.JobId
						};

						await _db.PartyInviteEntries.AddAsync(entity);
						await _db.SaveChangesAsync();
					}
					
					transaction.Commit();
				}
				catch (Exception e)
				{
					transaction.Rollback();
					await ReplyAsync(e.Message);
					return;
				}
			}

			await ReplyAsync($"#{invite.PartyInviteId} synced!");
		}
		
		[Command("invite-r")]
		public async Task RefreshInvite(int id)
		{
			var invite = await _db.PartyInvites.FindAsync(id);
			if (invite == null) return;

			var message = await Context.Channel.GetMessageAsync(invite.MessageId) as IUserMessage;
			if (message?.Embeds.Any() != true) return;

			var jobs = _db.CharacterJobs.ToList();
			var emotes = Context.Guild.Emotes.Where(x => jobs.Select(y => y.ReactionName).Contains(x.Name)).ToList();

			var entries = invite.PartyInviteEntries.ToList();

			var users = new List<(IUser user, GuildEmote emote)>();
			foreach (var emote in emotes)
			{
				await foreach (var user in message.GetReactionUsersAsync(emote, 20))
				{
					users.AddRange(user.Select(x => (x, emote)));
				}
			}

			using (var transaction = await _db.Database.BeginTransactionAsync())
			{
				try
				{
					foreach (var entry in entries)
					{
						if (!users.Any(x => x.user.Id == entry.UserId))
						{
							_db.Remove(entry);
							await _db.SaveChangesAsync();
							continue;
						}

						var jobUser = users.FirstOrDefault(x => x.user.Id == entry.UserId);
						var job = jobs.First(x => x.ReactionName == jobUser.emote.Name);
						if (entry.CharacterJobId != job.JobId)
						{
							entry.CharacterJobId = job.JobId;
							entry.ReactionName = jobUser.emote.Name;

							_db.Update(entry);
							await _db.SaveChangesAsync();
							continue;
						}

						var entity = new PartyInviteEntry
						{
							PartyInviteId = invite.PartyInviteId,
							ReactionName = jobUser.emote.Name,
							UserId = jobUser.user.Id,
							CharacterJobId = job.JobId
						};

						await _db.PartyInviteEntries.AddAsync(entry);
						await _db.SaveChangesAsync();
					}

					await transaction.CommitAsync();
				}
				catch (Exception)
				{
					transaction.Rollback();
					return;
				}
			}

			var inviter = await Context.Channel.GetUserAsync(invite.UserId);

			await message.ModifyAsync(properties =>
			{
				properties.Embed = CreateEmbed(message, invite, inviter);
			});
		}
		
		private Embed CreateEmbed(IUserMessage message, PartyInvite invite, IUser user)
		{
			var template = invite.RaidTemplate;
			var contentName = !string.IsNullOrEmpty(template.QuestUrl)
				? $"[{template.RaidName}]({template.QuestUrl})"
				: template.RaidName;

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
			var template = invite.RaidTemplate;
			var contentName = !string.IsNullOrEmpty(template.QuestUrl)
				? $"[{template.RaidName}]({template.QuestUrl})"
				: template.RaidName;

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