﻿using System;
using System.Threading;
using System.Threading.Tasks;
using BastionModerateApp.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BastionModerateApp
{
	internal class Program
	{
		private static void Main(string[] args) => ConfigureHostBuilder(args).Build().Run();

		public static IHostBuilder ConfigureHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
			.ConfigureLogging(b => b.AddConsole())
			.ConfigureServices((context, services) =>
			{
				services.AddHostedService<BotService>();
			});
	}

	internal class BotService : BackgroundService
	{
		private readonly IConfiguration _configuration;
		private readonly DiscordSocketClient _client;

		/// <summary>
		/// Discord BOTクライアントを作成します.
		/// </summary>
		/// <param name="configuration"></param>
		public BotService(IConfiguration configuration)
		{
			_configuration = configuration;
			
			_client = new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity.Info});
		}

		/// <inheritdoc />
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			BastionContext.IsMigration = false;
			
			var services = ConfigureServices();
			services.GetRequiredService<LogService>();
			await services.GetRequiredService<CommandHandlingService>().InstallCommandsAsync(services);
			services.GetRequiredService<AccountService>();
			services.GetRequiredService<ReactionHandler>();
			
			await _client.LoginAsync(TokenType.Bot, _configuration.GetSection("Discord").GetValue<string>("Token"));
			await _client.StartAsync();
		}


		/// <inheritdoc />
		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			await _client.StopAsync();
		}

		private IServiceProvider ConfigureServices()
		{
			return new ServiceCollection()
				.AddHttpClient()
				.AddSingleton(_client)
				.AddSingleton<CommandService>()
				.AddSingleton<CommandHandlingService>()
				.AddSingleton<AccountService>()
				// Log service
				.AddLogging(opt =>
				{
					opt.AddConsole();
				})
				.AddSingleton<LogService>()
				// Extra
				.AddSingleton(_configuration)
				// Add additional services here...
				.AddDbContext<BastionContext>(builder =>
				{
					builder.UseSqlite(_configuration.GetConnectionString("Default"));
					builder.UseLazyLoadingProxies();
				})
				.AddSingleton<ReactionHandler>()
				.BuildServiceProvider();
		}
	}
}