using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
		
		[Command("register")]
		public async Task RegisterAsync(int id)
		{
			var portal = _configuration.GetValue<string>("Portal");
			var baseUrl = _configuration.GetValue<string>("API");
			var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/character/{id}");
			var client = _clientFactory.CreateClient();
			var response = await client.SendAsync(request);

			if (!response.IsSuccessStatusCode) return;
			
			var content = await response.Content.ReadAsStringAsync();
			var json = JsonSerializer.Deserialize<dynamic>(content);

			var name = json["Character"]["Name"];
			var characterId = (long) json["Character"]["ID"];

			using (var transaction = await _db.Database.BeginTransactionAsync())
			{
				try
				{
					if (_db.Users.Any(x => x.DiscordId == Context.User.Id))
					{
						await transaction.RollbackAsync();
						await ReplyAsync("It has already been registered.");
						return;
					}

					var entity = new User
					{
						DiscordId = Context.User.Id,
						PlayerName = name,
						PlayerId = characterId
					};

					await _db.AddAsync(entity);

					await _db.SaveChangesAsync();

					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					throw;
				}
			}
			
			var builder = new StringBuilder()
				.AppendLine("Your registration is complete.")
				.AppendLine($"ID: {characterId} {name}");

			await ReplyAsync(builder.ToString());
		}

		[Command("unregister")]
		public async Task UnregisterAsync()
		{
			var user = await _db.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id);
			if (user == null)
			{
				await ReplyAsync("You are not registered.");
				return;
			}

			using (var transaction = await _db.Database.BeginTransactionAsync())
			{
				try
				{
					_db.Users.Remove(user);

					await _db.SaveChangesAsync();

					await transaction.CommitAsync();
				}
				catch (Exception)
				{
					await transaction.RollbackAsync();
					throw;
				}
			}

			await ReplyAsync("The deletion is complete.");
		}
	}
}