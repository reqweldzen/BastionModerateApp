using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BastionModerateApp.Services
{
	public class CommandHandlingService
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private IServiceProvider _provider;
		private IConfiguration _configuration;

		public CommandHandlingService(DiscordSocketClient client, CommandService commands)
		{
			_client = client;
			_commands = commands;
			
			_client.MessageReceived += MessageReceivedAsync;
		}

		public async Task InstallCommandsAsync(IServiceProvider provider)
		{
			_provider = provider;
			_configuration = provider.GetService<IConfiguration>();

			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), services: null);
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
			var prefix = _configuration?.GetValue<string>("Prefix") ?? "!";
			if (!(message.HasStringPrefix(prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
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