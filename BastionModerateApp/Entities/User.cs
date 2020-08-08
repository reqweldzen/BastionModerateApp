using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 8618

namespace BastionModerateApp.Entities
{
	/// <summary>
	/// ユーザー
	/// </summary>
	[Table("users", Schema = "transaction")]
	public class User
	{
		/// <summary>
		/// ユーザーID
		/// </summary>
		public int UserId { get; set; }
		
		/// <summary>
		/// Discord ユーザーID
		/// </summary>
		public ulong DiscordId { get; set; }
		
		/// <summary>
		/// プレイヤーID
		/// </summary>
		public long PlayerId { get; set; }
		
		/// <summary>
		/// プレイヤー名
		/// </summary>
		public string PlayerName { get; set; }
	}
}