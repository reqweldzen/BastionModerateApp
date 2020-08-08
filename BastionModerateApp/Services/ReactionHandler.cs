using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BastionModerateApp.Entities;
using BastionModerateApp.Enums;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BastionModerateApp.Services
{
	public class ReactionHandler
	{
		private readonly BastionContext _db;
		private readonly DiscordSocketClient _client;

		public ReactionHandler(DiscordSocketClient client, BastionContext db)
		{
			_db = db;
			_client = client;

			_client.ReactionAdded += ReactionAdded;
			_client.ReactionRemoved += ReactionRemoved;
		}

		private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel,
			SocketReaction reaction)
		{
			if (!reaction.User.IsSpecified) return;

			var user = await _db.Users.FirstOrDefaultAsync(x => x.DiscordId == reaction.UserId);
			if (user == null)
			{
				await channel.SendMessageAsync("You are not registered.");
				return;
			}

			var job = await _db.Jobs.FirstOrDefaultAsync(x => x.ShortcutName == reaction.Emote.Name);
			if (job == null)
			{
				return;
			}

			var message = await cache.DownloadAsync();
			var embeds = message.Embeds.ToList();
			if (embeds.Any())
			{
				var first = embeds[0];
				if (!int.TryParse(first.Description[1..], out var inviteId))
				{
					await channel.SendMessageAsync("Unable to get the invitation ID.");
					return;
				}

				var invite = await _db.PartyInvites.FindAsync(inviteId);
				if (invite == null)
				{
					await channel.SendMessageAsync("The invitation ID is invalid.");
					return;
				}

				// if (invite.PartyInviteEntries.Any(x => x.User.DiscordId == reaction.UserId))
				// {
				// 	await message.RemoveReactionAsync(reaction.Emote, reaction.UserId,
				// 		new RequestOptions {RetryMode = RetryMode.RetryRatelimit});
				// 	return;
				// }

				var entry = new PartyInviteEntry
				{
					PartyInviteId = invite.PartyInviteId,
					JobId = job.JobId,
					UserId = user.UserId,
				};

				await _db.PartyInviteEntries.AddAsync(entry);
				await _db.SaveChangesAsync();

				var inviter = await channel.GetUserAsync(invite.User.DiscordId);

				await message.ModifyAsync(properties => { properties.Embed = CreateEmbed(invite, inviter); });
			}
		}

		private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel,
			SocketReaction reaction)
		{
			if (!reaction.User.IsSpecified) return;

			var user = await _db.Users.FirstOrDefaultAsync(x => x.DiscordId == reaction.UserId);
			if (user == null)
			{
				await channel.SendMessageAsync("You are not registered.");
				return;
			}

			var job = await _db.Jobs.FirstOrDefaultAsync(x => x.ShortcutName == reaction.Emote.Name);
			if (job == null)
			{
				return;
			}

			var message = await cache.DownloadAsync();
			var embeds = message.Embeds.ToList();
			if (embeds.Any())
			{
				var first = embeds[0];

				if (!int.TryParse(first.Description[1..], out var inviteId))
				{
					await channel.SendMessageAsync("Unable to get the invitation ID.");
					return;
				}

				var invite = await _db.PartyInvites.FindAsync(inviteId);
				if (invite == null)
				{
					await channel.SendMessageAsync("The invitation ID is invalid.");
					return;
				}

				if (invite.IsFinished) return;

				var entry = invite.PartyInviteEntries.FirstOrDefault(x => x.UserId == user.UserId);
				if (entry == null) return;

				if (entry.JobId == job.JobId)
				{
					_db.PartyInviteEntries.Remove(entry);
					await _db.SaveChangesAsync();

					var inviter = await channel.GetUserAsync(invite.User.DiscordId);

					await message.ModifyAsync(properties => { properties.Embed = CreateEmbed(invite, inviter); });
				}
			}
		}

		private Embed CreateEmbed(PartyInvite invite, IUser inviter)
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
					builder.Name = template.ContentType.TypeName;
					builder.Value = contentName;
				})
				.WithFields(
					new EmbedFieldBuilder().WithName("目的").WithValue(invite.Purpose.DisplayName()).WithIsInline(true),
					new EmbedFieldBuilder().WithName("参加人数").WithValue($"{invite.PartyInviteEntries.Count}/8 人")
						.WithIsInline(true),
					new EmbedFieldBuilder().WithName("\u200b").WithValue("\u200b")
				)
				.WithFields(
					new EmbedFieldBuilder().WithName("開始日時").WithValue(invite.StartDate.ToString("yyyy/MM/dd HH:mm"))
						.WithIsInline(true),
					new EmbedFieldBuilder().WithName("終了日時")
						.WithValue(invite.EndDate?.ToString("yyyy/MM/dd HH:mm") ?? "N/A").WithIsInline(true)
				)
				.WithAuthor(inviter)
				.WithColor(Color.Purple)
				.WithCurrentTimestamp()
				.Build();
		}
	}
}