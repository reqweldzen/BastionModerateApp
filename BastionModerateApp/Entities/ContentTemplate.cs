using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 8618

namespace BastionModerateApp.Entities
{
	/// <summary>
	/// コンテンツテンプレート
	/// </summary>
	[Table("content_templates", Schema = "master")]
	public class ContentTemplate
	{
		/// <summary>
		/// コンテンツテンプレートID
		/// </summary>
		public int ContentTemplateId { get; set; }
		
		/// <summary>
		/// コンテンツ種別ID
		/// </summary>
		public int ContentTypeId { get; set; }
		
		/// <summary>
		/// コンテンツ名
		/// </summary>
		public string ContentName { get; set; }
		
		/// <summary>
		/// クエストURL
		/// </summary>
		public string? QuestUrl { get; set; }
		
		/// <summary>
		/// ショートカット名
		/// </summary>
		public string ShortcutName { get; set; }
		
		/// <summary>
		/// 参加可能人数
		/// </summary>
		public int MaxPlayer { get; set; }
		
		/// <summary>
		/// コンテンツ種別
		/// </summary>
		public virtual ContentType ContentType { get; set; }

		public string GetQuestName()
		{
			if (!string.IsNullOrEmpty(QuestUrl))
			{
				return $"[{ContentName}]({QuestUrl})";
			}

			return ContentName;
		}
	}
}