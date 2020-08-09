using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BastionModerateApp.Entities;
using BastionModerateApp.Models;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using JsonSerializer = Utf8Json.JsonSerializer;

namespace BastionModerateApp.Modules
{
	[Group("player")]
	public class PlayerModule : ModuleBase
	{
		private readonly IHttpClientFactory _clientFactory;
		private readonly IConfiguration _configuration;
		private readonly BastionContext _db;

		public PlayerModule(IHttpClientFactory clientFactory, IConfiguration configuration, BastionContext db)
		{
			_clientFactory = clientFactory;
			_configuration = configuration;
			_db = db;
		}

		/// <summary>
		/// プレイヤー登録
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Command("register")]
		public async Task RegisterAsync(int id)
		{
			var character = await GetCharacterAsync(_clientFactory, _configuration, id);
			if (character == null)
				return;

			if (_db.Users.Any(x => x.DiscordId == Context.User.Id))
			{
				await ReplyAsync("It has already been registered.");
				return;
			}

			var entity = new User
			{
				DiscordId = Context.User.Id,
				PlayerName = character.Character.Name,
				PlayerId = character.Character.ID
			};

			await _db.AddAsync(entity);

			await _db.SaveChangesAsync();

			var builder = new StringBuilder()
				.AppendLine("Your registration is complete.")
				.AppendLine($"ID: {character.Character.ID} {character.Character.Name}");

			await ReplyAsync(builder.ToString());
		}

		/// <summary>
		/// プレイヤー変更
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Command("change")]
		public async Task ChangeAsync(int id)
		{
			var user = await _db.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id);
			if (user == null)
			{
				await ReplyAsync("You are not registered.");
				return;
			}

			var character = await GetCharacterAsync(_clientFactory, _configuration, id);
			if (character == null)
				return;

			user.PlayerId = character.Character.ID;
			user.PlayerName = character.Character.Name;

			await _db.SaveChangesAsync();

			var builder = new StringBuilder()
				.AppendLine("Your registration is complete.")
				.AppendLine($"ID: {character.Character.ID} {character.Character.Name}");

			await ReplyAsync(builder.ToString());
		}

		/// <summary>
		/// プレイヤー登録解除
		/// </summary>
		/// <returns></returns>
		[Command("unregister")]
		public async Task UnregisterAsync()
		{
			var user = await _db.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id);
			if (user == null)
			{
				await ReplyAsync("You are not registered.");
				return;
			}

			_db.Users.Remove(user);

			await _db.SaveChangesAsync();

			await ReplyAsync("The deletion is complete.");
		}
		
		private static async Task<GameCharacter?> GetCharacterAsync(IHttpClientFactory factory,
			IConfiguration configuration, int id)
		{
			var baseUri = configuration.GetValue<string>("API");
			var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUri}/character/{id}");
			var client = factory.CreateClient();
			var response = await client.SendAsync(request);

			if (!response.IsSuccessStatusCode) return null;

			var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<GameCharacter>(content);
		}
	}
}