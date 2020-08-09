using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BastionModerateApp.Services
{
	public class LogService
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger _discordLogger;
		private readonly ILogger _commandsLogger;
		
		public LogService(DiscordSocketClient client, CommandService commands, ILoggerFactory loggerFactory)
		{
			_client = client;
			_commands = commands;
			_loggerFactory = loggerFactory;

			_discordLogger = _loggerFactory.CreateLogger("discord");
			_commandsLogger = _loggerFactory.CreateLogger("commands");

			_client.Log += LogDiscord;
			_commands.Log += LogCommand;
		}

		private Task LogDiscord(LogMessage message)
		{
			_discordLogger.Log(
				LogLevelFromSeverity(message.Severity),
				0,
				message,
				message.Exception,
				(_, __) => message.ToString(prependTimestamp: false)
			);

			return Task.CompletedTask;
		}

		private Task LogCommand(LogMessage message)
		{
			if (message.Exception is CommandException command)
			{
				// Don't risk blocking the logging task by awaiting a message send; ratelimits!?
				var _ = command.Context.Channel.SendMessageAsync($"Error: {command.Message}");
			}
			
			_commandsLogger.Log(
				LogLevelFromSeverity(message.Severity),
				0,
				message,
				message.Exception,
				(_, __) => message.ToString(prependTimestamp: false));

			return Task.CompletedTask;
		}

		private static LogLevel LogLevelFromSeverity(LogSeverity severity)
			=> (LogLevel) Math.Abs((int) severity - 5);
	}
}