using BastionModerateApp.Enums;

namespace BastionModerateApp.Models
{
	public class Character
	{
		public string Bio { get; set; }
		public string DC { get; set; }
		public Gender Gender { get; set; }
		public long ID { get; set; }
		public string Name { get; set; }
		public string Nameday { get; set; }
		public string Portrait { get; set; }
	}
}