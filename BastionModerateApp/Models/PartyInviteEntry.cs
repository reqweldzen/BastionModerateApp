using System.ComponentModel.DataAnnotations;

namespace BastionModerateApp.Models
{
	public class PartyInviteEntry
	{
		public int PartyInviteId { get; set; }
		
		public int PartyInviteEntryId { get; set; }

		public int CharacterJobId { get; set; }
		
		public ulong UserId { get; set; }
		
		public string ReactionName { get; set; }

		public virtual PartyInvite PartyInvite { get; set; }
	}
}