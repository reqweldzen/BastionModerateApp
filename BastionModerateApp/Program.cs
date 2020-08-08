using System;
using System.Reflection;
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
		private DiscordSocketClient _client;
		private CommandService _commands;
		private IServiceProvider _provider;

		public BotService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			BastionContext.IsMigration = false;
			
			_client = new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity.Info});

			var services = ConfigureServices();
			services.GetRequiredService<LogService>();
			await services.GetRequiredService<CommandHandlingService>().InstallCommandsAsync(services);
			
			await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
			await _client.StartAsync();
		}
		
		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			await _client.StopAsync();
		}

		private IServiceProvider ConfigureServices()
		{
			return new ServiceCollection()
				.AddSingleton(_client)
				.AddSingleton<CommandService>()
				.AddSingleton<CommandHandlingService>()
				// Extra
				.AddSingleton(_configuration)
				// Add additional services here...
				.AddDbContext<BastionContext>(builder =>
				{
					builder.UseNpgsql(_configuration.GetConnectionString("Default"));
					builder.UseLazyLoadingProxies();
					builder.UseSnakeCaseNamingConvention();
				})
				.AddSingleton<ReactionHandler>()
				.BuildServiceProvider();
		}
	}
}