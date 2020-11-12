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
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;

namespace BastionModerateApp.Services
{
	public class ReactionHandler
	{
		private readonly BastionContext _db;
		private readonly IConfiguration _configuration;
		private readonly DiscordSocketClient _client;

		/// <summary>
		/// MessageのReactionをハンドリングします。
		/// </summary>
		/// <param name="client"></param>
		/// <param name="db"></param>
		/// <param name="configuration"></param>
		public ReactionHandler(DiscordSocketClient client, BastionContext db, IConfiguration configuration)
		{
			_db = db;
			_configuration = configuration;
			_client = client;

			_client.ReactionAdded += ReactionAdded;
			_client.ReactionRemoved += ReactionRemoved;
			_client.ReactionsCleared += ReactionsCleared;
		}

		/// <summary>
		/// リアクション全削除時
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		private async Task ReactionsCleared(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel)
		{
			await using var transaction = await _db.Database.BeginTransactionAsync();
			
			var message = await cache.DownloadAsync();

			var embeds = message.Embeds.ToList();
			if (!embeds.Any()) return;

			var inviteEmbed = new InviteEmbedWrapper(embeds[0].ToEmbedBuilder());

			if (inviteEmbed.InviteId == null)
			{
				return;
			}

			var invite = await _db.PartyInvites.FindAsync(inviteEmbed.InviteId);
			if (invite == null)
			{
				await channel.SendMessageAsync("The invitation ID is invalid.");
				return;
			}
			
			if (!(await channel.GetMessageAsync(invite.MemberListMessageId) is IUserMessage memberMessage)) return;

			if (invite.IsFinished) return;

			var entries = invite.PartyInviteEntries.ToList();
			if (!entries.Any()) return;

			_db.PartyInviteEntries.RemoveRange(entries);
			await _db.SaveChangesAsync();

			var split = memberMessage.Content.Split("\r\n").ToList();

			inviteEmbed.CurrentPlayer = 0;

			await message.ModifyAsync(properties => properties.Embed = inviteEmbed.ToEmbed());

			await memberMessage.ModifyAsync(properties => properties.Content = string.Join("\r\n", split[0]));

			await transaction.CommitAsync();
		}
		
		/// <summary>
		/// リアクション追加時
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="channel"></param>
		/// <param name="reaction"></param>
		/// <returns></returns>
		private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel,
			SocketReaction reaction)
		{
			if (!reaction.User.IsSpecified) return;

			if (channel.Id == _configuration.GetSection("ReactionChannel").GetValue<ulong>("Beginner"))
			{
				await OnAddBeginner(cache, channel, reaction);
				return;
			}
			
			await using var transaction = await _db.Database.BeginTransactionAsync();

			var user = await _db.Users.FirstOrDefaultAsync(x => x.DiscordId == reaction.UserId);
			if (user == null)
			{
				await channel.SendMessageAsync("You are not registered.");
				return;
			}

			var job = await _db.Jobs.FirstOrDefaultAsync(x => x.ShortcutName == reaction.Emote.Name);
			if (job == null) return;

			var message = await cache.DownloadAsync();
			var embeds = message.Embeds.ToList();
			if (!embeds.Any()) return;

			var inviteEmbed = new InviteEmbedWrapper(embeds[0].ToEmbedBuilder());
			if (inviteEmbed.InviteId == null)
			{
				await channel.SendMessageAsync("Unable to get the invitation ID.");
				return;
			}

			var invite = await _db.PartyInvites.FindAsync(inviteEmbed.InviteId);
			if (invite == null)
			{
				await channel.SendMessageAsync("The invitation ID is invalid.");
				return;
			}

			if (!(await channel.GetMessageAsync(invite.MemberListMessageId) is IUserMessage memberMessage)) return;

			if (_db.PartyInviteEntries.Any(x => x.PartyInviteId == invite.PartyInviteId && 
				x.User.DiscordId == reaction.UserId))
			{
				await message.RemoveReactionAsync(reaction.Emote, reaction.UserId,
					new RequestOptions {RetryMode = RetryMode.RetryRatelimit});
				return;
			}

			var entry = new PartyInviteEntry
			{
				PartyInviteId = invite.PartyInviteId,
				JobId = job.JobId,
				UserId = user.UserId,
			};

			await _db.PartyInviteEntries.AddAsync(entry);
			await _db.SaveChangesAsync();


			var split = memberMessage.Content.Split("\r\n").ToList();
			split.Add($"{reaction.Emote} {entry.User.PlayerName}");
			
			inviteEmbed.CurrentPlayer = invite.PartyInviteEntries.Count;

			await message.ModifyAsync(properties => properties.Embed = inviteEmbed.ToEmbed());

			await memberMessage.ModifyAsync(properties => properties.Content = string.Join("\r\n", split));

			await transaction.CommitAsync();
		}

		/// <summary>
		/// リアクション削除時
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="channel"></param>
		/// <param name="reaction"></param>
		/// <returns></returns>
		private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel,
			SocketReaction reaction)
		{
			if (!reaction.User.IsSpecified) return;

			if (channel.Id == _configuration.GetSection("ReactionChannel").GetValue<ulong>("Beginner"))
			{
				await OnRemoveBeginner(cache, channel, reaction);
				return;
			}

			return;

			await using var transaction = await _db.Database.BeginTransactionAsync();

			var user = await _db.Users.FirstOrDefaultAsync(x => x.DiscordId == reaction.UserId);
			if (user == null)
			{
				return;
			}

			var job = await _db.Jobs.FirstOrDefaultAsync(x => x.ShortcutName == reaction.Emote.Name);
			if (job == null)
			{
				return;
			}

			var message = await cache.DownloadAsync();

			var embeds = message.Embeds.ToList();
			if (!embeds.Any()) return;

			var inviteEmbed = new InviteEmbedWrapper(embeds[0].ToEmbedBuilder());

			if (inviteEmbed.InviteId == null)
			{
				await channel.SendMessageAsync("Unable to get the invitation ID.");
				return;
			}

			var invite = await _db.PartyInvites.FindAsync(inviteEmbed.InviteId);
			if (invite == null)
			{
				await channel.SendMessageAsync("The invitation ID is invalid.");
				return;
			}
			
			if (!(await channel.GetMessageAsync(invite.MemberListMessageId) is IUserMessage memberMessage)) return;

			if (invite.IsFinished) return;

			var entry = invite.PartyInviteEntries.FirstOrDefault(x => x.UserId == user.UserId);
			if (entry == null) return;

			if (entry.JobId != job.JobId) return;
			
			_db.PartyInviteEntries.Remove(entry);
			await _db.SaveChangesAsync();

			var split = memberMessage.Content.Split("\r\n").ToList();
			split.RemoveAll(x => x.Contains(entry.User.PlayerName));
			
			inviteEmbed.CurrentPlayer = invite.PartyInviteEntries.Count;

			await message.ModifyAsync(properties => properties.Embed = inviteEmbed.ToEmbed());

			await memberMessage.ModifyAsync(properties => properties.Content = string.Join("\r\n", split));

			await transaction.CommitAsync();
		}
		
		#region Put Mark (Beginner)
		
		private async Task OnAddBeginner(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
		{
			if (reaction.Emote.Name != "🔰")
			{
				return;
			}
			
			if (reaction.User.Value is IGuildUser user)
			{
				var name = user.Nickname;
				await user.ModifyAsync(guildUser =>
				{
					var before = name;
					guildUser.Nickname = $"🔰{before}";
				});
			}
		}

		private async Task OnRemoveBeginner(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel,
			SocketReaction reaction)
		{
			if (reaction.Emote.Name != "🔰")
			{
				return;
			}

			if (reaction.User.Value is IGuildUser user)
			{
				var name = user.Nickname;
				await user.ModifyAsync(guildUser =>
				{
					var before = name;
					var after = before.Replace("🔰", "");
					guildUser.Nickname = $"{after}";
				});
			}
		}
		
		#endregion
	}
}