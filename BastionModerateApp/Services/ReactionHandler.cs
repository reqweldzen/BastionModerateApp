using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BastionModerateApp.Enums;
using BastionModerateApp.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BastionModerateApp.Services
{
	public class ReactionHandler
	{
		private readonly IServiceProvider _services;
		private readonly BastionContext _context;
		private readonly DiscordSocketClient _client;
		private readonly IReadOnlyList<CharacterJob> _characterJobs;

		public ReactionHandler(IServiceProvider services, BastionContext context, DiscordSocketClient client)
		{
			_services = services;
			_context = context;
			_client = client;

			_characterJobs = _context.CharacterJobs.ToList();
			
			_client.ReactionAdded += ClientOnReactionAdded;
			_client.ReactionRemoved += ClientOnReactionRemoved;
		}

		private async Task ClientOnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel,
			SocketReaction reaction)
		{
			if (!reaction.User.IsSpecified) return;

			var message = await cache.DownloadAsync();
			if (message.Embeds.Any())
			{
				var embed = message.Embeds.First();
				if (int.TryParse(embed.Description[1..], out var inviteId))
				{
					var invite = await _context.PartyInvites.FindAsync(inviteId);
					if (invite != null)
					{
						if (_context.PartyInviteEntries.Any(x =>
							x.PartyInviteId == invite.PartyInviteId && x.UserId == reaction.UserId))
						{
							return;
						}

						if (_characterJobs.Any(x => x.ReactionName == reaction.Emote.Name))
						{
							var job = _characterJobs.First(x => x.ReactionName == reaction.Emote.Name);
							
							var entry = new PartyInviteEntry
							{
								PartyInviteId = invite.PartyInviteId,
								ReactionName = reaction.Emote.Name,
								UserId = reaction.UserId,
								CharacterJobId = job.CharacterJobId
							};

							await _context.PartyInviteEntries.AddAsync(entry);
							await _context.SaveChangesAsync();

							var user = await channel.GetUserAsync(invite.UserId);

							await message.ModifyAsync(properties =>
							{
								properties.Embed = CreateEmbed(message, invite, user);
							});
						}
					}
				}
			}
		}

		private async Task ClientOnReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel,
			SocketReaction reaction)
		{
			if (!reaction.User.IsSpecified) return;

			var message = await cache.DownloadAsync();
			if (message.Embeds.Any())
			{
				var embed = message.Embeds.First();
				if (int.TryParse(embed.Description[1..], out var inviteId))
				{
					var invite = await _context.PartyInvites.FindAsync(inviteId);
					if (invite != null)
					{
						var entry = await _context.PartyInviteEntries
							.FirstOrDefaultAsync(x =>
								x.PartyInviteId == invite.PartyInviteId &&
								x.UserId == reaction.UserId);
						if (entry != null)
						{
							if (reaction.Emote.Name == entry.ReactionName)
							{
								_context.PartyInviteEntries.Remove(entry);
								await _context.SaveChangesAsync();

								var user = await channel.GetUserAsync(invite.UserId);

								await message.ModifyAsync(properties =>
								{
									properties.Embed = CreateEmbed(message, invite, user);
								});
							}
						}
					}
				}
			}
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
	}
}