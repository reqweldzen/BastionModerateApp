using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BastionModerateApp.Commands
{
	public class Dice : ModuleBase
	{
		/// <summary>
		/// Sample from https://qiita.com/xxIN0xx/items/9cb561f1765d427957a9
		/// </summary>
		/// <param name="face"></param>
		/// <param name="throwCount"></param>
		/// <returns></returns>
		[Command("dice"), AliasAttribute("dicethrow")]
		public async Task DiceThrow(byte face = 6, byte throwCount = 1)
		{
			if (face < 1)
			{
				await ReplyAsync("face less than 1");
				return;
			}

			if (throwCount < 1)
			{
				await ReplyAsync("throw count less than 1");
				return;
			}

			var resultText = new StringBuilder();
			var firstLine = true;
			var summary = 0;
			for (int i = 0; i < throwCount; i++)
			{
				var result = new Random().Next(1, face + 1);
				resultText.Append(firstLine ? result.ToString().PadLeft(3) : $", {result.ToString().PadLeft(3)}");
				summary += result;

				firstLine = false;
			}
			
			var embed = new EmbedBuilder();
			embed.WithTitle("Dice Result");
			embed.WithDescription(resultText.ToString());

			await ReplyAsync(
				$"{face}d{throwCount}\r\nTotal:{summary}, Average:{((double) summary / throwCount):#,0.00}", embed: embed.Build());
		}
	}
}