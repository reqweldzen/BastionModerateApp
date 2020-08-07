using System.ComponentModel.DataAnnotations;

namespace BastionModerateApp.Models
{
	public class RaidTemplate
	{
		public int RaidTemplateId { get; set; }
		
		public string RaidName { get; set; }
		
		public string ShortcutName { get; set; }
		
		public string QuestUrl { get; set; }
	}
}