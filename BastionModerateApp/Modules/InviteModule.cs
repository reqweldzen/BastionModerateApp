using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BastionModerateApp.Entities;
using BastionModerateApp.Enums;
using BastionModerateApp.Models;
using BastionModerateApp.Services;
using Discord;
using Discord.Commands;

// ReSharper disable UnusedMember.Global

namespace BastionModerateApp.Modules
{
	/// <summary>
	/// パーティ募集モジュール
	/// </summary>
	[Group("invite")]
	public class InviteModule : ModuleBase
	{
		private readonly BastionContext _db;
		private readonly AccountService _accountService;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="db"></param>
		/// <param name="accountService"></param>
		public InviteModule(BastionContext db, AccountService accountService)
		{
			_db = db;
			_accountService = accountService;
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

		/// <summary>
		/// パーティ募集を作成する。
		/// </summary>
		/// <param name="shortcutName"></param>
		/// <param name="purpose"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		[Command("create")]
		public async Task CreatePartyFinderAsync(string? shortcutName = null, int purpose = 1, DateTime? startDate = null,
			DateTime? endDate = null)
		{
			bool IsRegisteredUser(User x) => x.DiscordId == Context.User.Id;

			// トランザクション開始
			await using var transaction = await _db.Database.BeginTransactionAsync();

			var template = _db.ContentTemplates.FirstOrDefault(x => x.ShortcutName == shortcutName);
			if (template == null) return;

			var user = await _db.Users.FirstOrDefaultAsync(IsRegisteredUser);
			if (user == null)
			{
				if (!await _accountService.RegisterAsync(Context))
				{
					await ReplyAsync("You are not registered.");
					return;
				}
			}

			user = await _db.Users.FirstOrDefaultAsync(IsRegisteredUser);

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

			var inviteEmbed = new InviteEmbedWrapper
			{
				InviteId = invite.PartyInviteId,
				ContentType = invite.ContentTemplate.ContentType.TypeName,
				ContentName = invite.ContentTemplate.GetQuestName(),
				Purpose = invite.Purpose.DisplayName(),
				MaxPlayer = invite.ContentTemplate.MaxPlayer,
				StartDate = startDate,
				EndDate = endDate,
				Inviter = Context.User,
			};

			var inviteMessage = await ReplyAsync(embed: inviteEmbed.ToEmbed());
			var memberMessage = await ReplyAsync("参加者 \uD83D\uDCCC");

			invite.MessageId = inviteMessage.Id;
			invite.MemberListMessageId = memberMessage.Id;
			_db.PartyInvites.Update(invite);
			await _db.SaveChangesAsync();

			await Context.Message.DeleteAsync();

			await transaction.CommitAsync();
		}

		/// <summary>
		/// パーティ募集を終了する。
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Command("finish")]
		public async Task FinishInviteAsync(int id)
		{
			await using var transaction = await _db.Database.BeginTransactionAsync();

			var invite = await _db.PartyInvites.FindAsync(id);
			if (invite == null) return;

			var message = await Context.Channel.GetMessageAsync(invite.MessageId) as IUserMessage;
			if (message?.Embeds.Any() != true) return;

			var inviter = await Context.Channel.GetUserAsync(invite.User.DiscordId);
			if (inviter == null) return;

			var embed = message.Embeds.ToList()[0];
			var inviteEmbed = new InviteEmbedWrapper(embed.ToEmbedBuilder());

			invite.IsFinished = true;
			_db.PartyInvites.Update(invite);
			await _db.SaveChangesAsync();

			inviteEmbed.Title += "(終了)";

			await message.ModifyAsync(properties => { properties.Embed = inviteEmbed.ToEmbed(); });

			await transaction.CommitAsync();
		}

		[Command("show")]
		public async Task ShowInvitedMember(int id)
		{
			var invite = await _db.PartyInvites.FindAsync(id);
			if (invite == null) return;

			var jobs = _db.Jobs.ToList();
			var emotes = Context.Guild.Emotes.Where(x => jobs.Select(y => y.ShortcutName).Contains(x.Name)).ToList();

			var entries = invite.PartyInviteEntries.ToList();

			var builder = new StringBuilder();
			builder.AppendLine($"#{invite.PartyInviteId} {invite.ContentTemplate.ContentName}");
			foreach (var entry in entries)
			{
				builder.AppendLine(
					$"{emotes.FirstOrDefault(x => x.Name == entry.Job.ShortcutName)} {entry.User.PlayerName}");
			}

			await ReplyAsync(builder.ToString());
		}

		/// <summary>
		/// パーティ募集情報を更新する。
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Command("update")]
		public async Task UpdateInviteAsync(int id)
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

		/// <summary>
		/// パーティ募集を削除する
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Command("delete")]
		public async Task DeleteInviteAsync(int id)
		{
			await using var transaction = await _db.Database.BeginTransactionAsync();

			var invite = await _db.PartyInvites.FindAsync(id);
			if (invite == null) return;

			var message = await Context.Channel.GetMessageAsync(invite.MessageId) as IUserMessage;
			if (message?.Embeds.Any() != true)
			{
				await ReplyAsync($"The invitation message was not found.");
				return;
			}

			_db.PartyInvites.Remove(invite);

			await _db.SaveChangesAsync();

			await message.DeleteAsync();

			await Context.Message.DeleteAsync();

			await transaction.CommitAsync();
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