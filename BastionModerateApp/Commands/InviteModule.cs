using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using BastionModerateApp.Enums;
using BastionModerateApp.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace BastionModerateApp.Commands
{
	public class InviteModule : ModuleBase
	{
		private readonly DiscordSocketClient _client;
		private readonly BastionContext _context;

		private Subject<(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)>
			_subject = new Subject<(Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction)>();

		public InviteModule(DiscordSocketClient client, BastionContext context)
		{
			_client = client;
			_context = context;
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
			var template = _context.RaidTemplates.FirstOrDefault(x => x.ShortcutName == shortcutName);
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

			await _context.PartyInvites.AddAsync(invite);
			await _context.SaveChangesAsync();

			var embed = CreateEmbed(template, invite);
			
			await ReplyAsync(embed: embed);
		}
	}
}