using System;
using System.Collections.Generic;
using System.Linq;
using BastionModerateApp.Entities;
using Discord;

namespace BastionModerateApp.Models
{
	public class InviteEmbedWrapper
	{
		private readonly EmbedBuilder _embedBuilder;

		public string Title
		{
			get => _embedBuilder.Title;
			set => _embedBuilder.Title = value;
		}

		public int? InviteId
		{
			get
			{
				var result = int.TryParse(_embedBuilder.Description[1..], out var id);
				return result ? id : null as int?;
			}
			set => _embedBuilder.Description = $"#{value}";
		}

		public string ContentType
		{
			get => _embedBuilder.Fields[0].Name;
			set => _embedBuilder.Fields[0].Name = value;
		}

		public string ContentName
		{
			get => _embedBuilder.Fields[0].Value?.ToString() ?? "";
			set => _embedBuilder.Fields[0].Value = value;
		}

		public string Purpose
		{
			get => _embedBuilder.Fields[1].Value?.ToString() ?? "";
			set => _embedBuilder.Fields[1].Value = value;
		}

		public DateTime? StartDate
		{
			get
			{
				var result = DateTime.TryParse(_embedBuilder.Fields[2].Value?.ToString(), out var dt);
				return result ? dt : null as DateTime?;
			}
			set => _embedBuilder.Fields[2].Value = value?.ToString("yyyy/MM/dd HH:mm") ?? "N/A";
		}

		public DateTime? EndDate
		{
			get
			{
				var result = DateTime.TryParse(_embedBuilder.Fields[3].Value?.ToString(), out var dt);
				return result ? dt : null as DateTime?;
			}
			set => _embedBuilder.Fields[3].Value = value?.ToString("yyyy/MM/dd HH:mm") ?? "N/A";
		}

		public int? CurrentPlayer
		{
			get
			{
				var result = int.TryParse(_embedBuilder.Fields[4].Value?.ToString()?[0..1], out var current);
				return result ? current : null as int?;
			}
			set
			{
				var str = (_embedBuilder.Fields[4].Value.ToString() ?? "0/0").Split('/');
				if (str.Length != 2) str = new[] {"0", "0"};
				_embedBuilder.Fields[4].Value = $"{value}/{str[1]}";
			}
		}

		public int? MaxPlayer
		{
			get
			{
				var result = int.TryParse(_embedBuilder.Fields[4].Value?.ToString()?[2..3], out var max);
				return result ? max : null as int?;
			}
			set
			{
				var str = (_embedBuilder.Fields[4].Value.ToString() ?? "0/0").Split('/');
				if (str.Length != 2) str = new[] {"0", "0"};

				_embedBuilder.Fields[4].Value = $"{str[0]}/{value}";
			}
		}

		public IUser Inviter
		{
			set => _embedBuilder.WithAuthor(value);
		}

		public InviteEmbedWrapper()
		{
			_embedBuilder = new EmbedBuilder
			{
				Title = "パーティ募集",
				Fields = new List<EmbedFieldBuilder>()
				{
					new EmbedFieldBuilder(),
					new EmbedFieldBuilder().WithName("目的").WithIsInline(true),
					new EmbedFieldBuilder().WithName("開始日時").WithValue("N/A").WithIsInline(true),
					new EmbedFieldBuilder().WithName("終了日時").WithValue("N/A").WithIsInline(true),
					new EmbedFieldBuilder().WithName("参加人数").WithValue("0/0"),
				}
			};
		}

		public InviteEmbedWrapper(EmbedBuilder embedBuilder)
		{
			_embedBuilder = embedBuilder;
		}

		public Embed ToEmbed() => _embedBuilder.Build();
	}
}