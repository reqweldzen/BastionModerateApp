using Discord;

namespace BastionModerateApp.Models
{
	public struct EmoteUser
	{
		public IUser User { get; set; }
		public IEmote Emote { get; set; }
	}
}