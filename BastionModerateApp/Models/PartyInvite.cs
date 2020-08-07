using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BastionModerateApp.Enums;

namespace BastionModerateApp.Models
{
	public class PartyInvite
	{
		public int PartyInviteId { get; set; }
		
		public ulong UserId { get; set; }
		
		public ulong MessageId { get; set; }
		
		public int RaidTemplateId { get; set; }
		
		public Purpose Purpose { get; set; }
		
		public DateTime StartDate { get; set; }
		
		public bool IsFinished { get; set; }
		
		public virtual RaidTemplate RaidTemplate { get; set; }
		
		public virtual ICollection<PartyInviteEntry> PartyInviteEntries { get; set; }
	}
}