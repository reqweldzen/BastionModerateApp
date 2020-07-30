using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BastionModerateApp
{
	internal class Program
	{
		private DiscordSocketClient _client;
		private CommandService _commands;
		private IServiceProvider _provider;
		
		private static void Main(string[] args)
		{
			new Program().MainAsync().GetAwaiter().GetResult();
		}
		
		public async Task MainAsync()
		{
			_provider = new ServiceCollection().BuildServiceProvider();
			_commands = new CommandService();
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
			
			_client = new DiscordSocketClient();

			_client.Log += LogAsync;
			_client.MessageReceived += MessageReceivedAsync;
			
			await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
			await _client.StartAsync();

			await Task.Delay(Timeout.Infinite);
		}

		private Task LogAsync(LogMessage log)
		{
			Console.WriteLine(log.ToString());
			return Task.CompletedTask;
		}

		private Task ReadyAsync()
		{
			Console.WriteLine($"{_client.CurrentUser} is connected!");

			return Task.CompletedTask;
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