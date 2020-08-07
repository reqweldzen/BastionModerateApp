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
			
			_provider = new ServiceCollection()
				.AddSingleton(_configuration)
				.AddSingleton(provider => new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity.Info}))
				
				.AddDbContext<BastionContext>(builder =>
				{
					builder.UseNpgsql(_configuration.GetConnectionString("Default"));
					builder.UseLazyLoadingProxies();
					builder.UseSnakeCaseNamingConvention();
				})
				.AddSingleton(provider => new ReactionHandler(provider, provider.GetService<BastionContext>(), provider.GetService<DiscordSocketClient>()))
				.BuildServiceProvider();
			
			_commands = new CommandService();
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

			var handler = _provider.GetService<ReactionHandler>();
			_client = _provider.GetService<DiscordSocketClient>();

			_client.Log += x =>
			{
				Console.WriteLine($"{x.Message}, {x.Exception}");
				return Task.CompletedTask;
			};

			_client.MessageReceived += MessageReceivedAsync;

			await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
			await _client.StartAsync();
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			await _client.StopAsync();
		}

		private async Task MessageReceivedAsync(SocketMessage messageParam)
		{
			var message = messageParam as SocketUserMessage;
			Console.WriteLine($"{message?.Channel.Name} {message?.Author.Username} {message}");

			if (message?.Author.IsBot ?? true)
			{
				return;
			}

			var argPos = 0;
			if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
			{
				return;
			}

			var context = new CommandContext(_client, message);
			var result = await _commands.ExecuteAsync(context, argPos, _provider);

			if (!result.IsSuccess)
			{
				await context.Channel.SendMessageAsync(result.ErrorReason);
			}
		}
	}
}