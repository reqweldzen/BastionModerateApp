using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BastionModerateApp.Models
{
	public class User
	{
		public int UserId { get; set; }
		
		public ulong DiscordId { get; set; }
		
		public string PlayerName { get; set; }
		
		public long PlayerId { get; set; }
	}
}