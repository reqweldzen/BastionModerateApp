namespace BastionModerateApp.Entities
{
	public class User
	{
		public int UserId { get; set; }
		
		public ulong DiscordId { get; set; }
		
		public string PlayerName { get; set; }
		
		public long PlayerId { get; set; }
	}
}