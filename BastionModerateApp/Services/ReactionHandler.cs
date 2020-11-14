using System.Linq;
using System.Threading.Tasks;
using BastionModerateApp.Entities;
using BastionModerateApp.Models;
using Discord;
using Discord.WebSocket;

namespace BastionModerateApp.Services
{
	public class ReactionHandler
	{
		private readonly AccountService _accountService;
		private readonly BastionContext _db;

		/// <summary>
		/// MessageのReactionをハンドリングします。
		/// </summary>
		/// <param name="client"></param>
		/// <param name="accountService"></param>
		/// <param name="db"></param>
		public ReactionHandler(DiscordSocketClient client, AccountService accountService, BastionContext db)
		{
			_accountService = accountService;
			_db = db;

			client.ReactionAdded += ReactionAdded;
			client.ReactionRemoved += ReactionRemoved;
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
			await using var transaction = await _db.Database.BeginTransactionAsync();

			var user = await _accountService.GetOrRegisterAsync(reaction.UserId);

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

			var guildUser = await ((IGuildChannel) channel).Guild.GetUserAsync(entry.User.DiscordId);
			var split = memberMessage.Content.Split("\r\n").ToList();
			split.Add($"{reaction.Emote} {guildUser.Nickname ?? $"{guildUser.Username}#{guildUser.Discriminator}"}");
			
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
			return;
			
			await using var transaction = await _db.Database.BeginTransactionAsync();

			var user = await _accountService.GetOrRegisterAsync(reaction.UserId);

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

			var guild = (channel as IGuildChannel)?.Guild;
			var split = memberMessage.Content.Split("\r\n").ToList();
			
			inviteEmbed.CurrentPlayer = invite.PartyInviteEntries.Count;

			await message.ModifyAsync(properties => properties.Embed = inviteEmbed.ToEmbed());

			await memberMessage.ModifyAsync(properties => properties.Content = string.Join("\r\n", split));

			await transaction.CommitAsync();
		}
	}
}