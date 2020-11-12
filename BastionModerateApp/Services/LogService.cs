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
		private readonly ILogger _discordLogger;
		private readonly ILogger _commandsLogger;
		
		/// <summary>
		/// Discordのログをハンドリングします。
		/// </summary>
		/// <param name="client"></param>
		/// <param name="commands"></param>
		/// <param name="loggerFactory"></param>
		public LogService(DiscordSocketClient client, CommandService commands, ILoggerFactory loggerFactory)
		{
			_discordLogger = loggerFactory.CreateLogger("discord");
			_commandsLogger = loggerFactory.CreateLogger("commands");

			client.Log += LogDiscordTask;
			commands.Log += LogCommandTask;
		}

		/// <summary>
		/// Discordメッセージをログに書き込む。
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private Task LogDiscordTask(LogMessage message)
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

		/// <summary>
		/// Discordコマンドをログに書き込む。
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private Task LogCommandTask(LogMessage message)
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