using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BastionModerateApp.Entities;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Internal;

namespace BastionModerateApp.Services
{
	public class AccountService
	{
		private readonly DiscordSocketClient _client;
		private readonly BastionContext _db;

		public AccountService(DiscordSocketClient client, BastionContext db)
		{
			_client = client;
			_db = db;
		}

		/// <summary>
		/// ユーザー登録をする。
		/// </summary>
		/// <returns></returns>
		public async Task<bool> RegisterAsync(ICommandContext context)
		{
			return false;
			if (_db.Users.Any(x => x.DiscordId == context.User.Id))
			{
				await context.Channel.SendMessageAsync("It has already been registered.");
				return false;
			}

			var entity = new User
			{
				DiscordId = context.User.Id,
				PlayerName = null,
				PlayerId = null
			};

			await _db.AddAsync(entity);

			await _db.SaveChangesAsync();

			return true;
		}
	}
}