// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable 8618

namespace BastionModerateApp.Entities
{
	/// <summary>
	/// パーティ募集エントリー
	/// </summary>
	[Table("party_invite_entries", Schema = "transaction")]
	public class PartyInviteEntry
	{
		/// <summary>
		/// パーティ募集ID
		/// </summary>
		public int PartyInviteId { get; set; }
		
		/// <summary>
		/// パーティ募集エントリーID
		/// </summary>
		public int PartyInviteEntryId { get; set; }

		/// <summary>
		/// ユーザーID
		/// </summary>
		public int UserId { get; set; }
		
		/// <summary>
		/// 職業ID
		/// </summary>
		public int JobId { get; set; }
		
		/// <summary>
		/// ユーザー
		/// </summary>
		public virtual User User { get; set; }
		
		/// <summary>
		/// 職業
		/// </summary>
		public virtual Job Job { get; set; }
		
		/// <summary>
		/// パーティ募集
		/// </summary>
		public virtual PartyInvite PartyInvite { get; set; }
	}
}