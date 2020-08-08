using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace BastionModerateApp.Services
{
	public class LogService
	{
		private readonly DiscordSocketClient _client;

		public LogService(DiscordSocketClient client)
		{
			_client = client;
		}

		public Task InstallLoggerAsync()
		{
			_client.Log += Log;
			return Task.CompletedTask;
		}

		private static Task Log(LogMessage logMessage)
		{
			Console.WriteLine($"{logMessage.Message}, {logMessage.Exception}");
			return Task.CompletedTask;
		}
	}
}