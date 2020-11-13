using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable 8618

namespace BastionModerateApp.Entities
{
	/// <summary>
	/// 職業
	/// </summary>
	[Table("jobs")]
	public class Job
	{
		/// <summary>
		/// 職業ID
		/// </summary>
		public int JobId { get; set; }
		
		/// <summary>
		/// 職業名
		/// </summary>
		public string JobName { get; set; }
		
		/// <summary>
		/// ショートカット名
		/// </summary>
		public string ShortcutName { get; set; }
	}
}