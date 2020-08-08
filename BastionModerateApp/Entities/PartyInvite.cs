using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BastionModerateApp.Enums;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
#pragma warning disable 8618

namespace BastionModerateApp.Entities
{
	/// <summary>
	/// パーティ募集
	/// </summary>
	[Table("party_invites", Schema = "transaction")]
	public class PartyInvite
	{
		/// <summary>
		/// パーティ募集ID
		/// </summary>
		public int PartyInviteId { get; set; }
		
		/// <summary>
		/// ユーザーID
		/// </summary>
		public int UserId { get; set; }
		
		/// <summary>
		/// メッセージID
		/// </summary>
		public ulong MessageId { get; set; }
		
		/// <summary>
		/// コンテンツテンプレートID
		/// </summary>
		public int ContentTemplateId { get; set; }
		
		/// <summary>
		/// 目的
		/// </summary>
		public Purpose Purpose { get; set; }
		
		/// <summary>
		/// 開始日時
		/// </summary>
		public DateTime StartDate { get; set; }
		
		/// <summary>
		/// 終了日時
		/// </summary>
		public DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 募集終了フラグ
		/// </summary>
		public bool IsFinished { get; set; }
		
		/// <summary>
		/// ユーザー
		/// </summary>
		public virtual User User { get; set; }
		
		/// <summary>
		/// コンテンツテンプレート
		/// </summary>
		public virtual ContentTemplate ContentTemplate { get; set; }
		
		/// <summary>
		/// パーティ募集エントリー
		/// </summary>
		public virtual ICollection<PartyInviteEntry> PartyInviteEntries { get; set; }
	}
}