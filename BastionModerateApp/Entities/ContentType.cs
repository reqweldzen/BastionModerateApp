using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable 8618
namespace BastionModerateApp.Entities
{
	/// <summary>
	/// コンテンツ種別
	/// </summary>
	[Table("content_types")]
	public class ContentType
	{
		/// <summary>
		/// コンテンツ種別ID
		/// </summary>
		public int ContentTypeId { get; set; }
		
		/// <summary>
		/// 種別名
		/// </summary>
		public string TypeName { get; set; }
	}
}