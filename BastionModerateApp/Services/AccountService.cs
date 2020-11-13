using System.Linq;
using System.Threading.Tasks;
using BastionModerateApp.Entities;

namespace BastionModerateApp.Services
{
	public class AccountService
	{
		private readonly BastionContext _db;

		public AccountService(BastionContext db)
		{
			_db = db;
		}

		/// <summary>
		/// ユーザー登録をする。
		/// </summary>
		/// <returns></returns>
		public async ValueTask<User> GetOrRegisterAsync(ulong userId)
		{
			var result = _db.Users.SingleOrDefault(x => x.DiscordId == userId);
			if (result != null)
			{
				return result;
			}

			var user = new User
			{
				DiscordId = userId
			};

			await _db.AddAsync(user);

			await _db.SaveChangesAsync();

			return user;
		}
	}
}