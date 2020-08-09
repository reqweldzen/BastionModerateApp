using System;
using System.ComponentModel;

namespace BastionModerateApp.Enums
{
	/// <summary>
	/// 目的
	/// </summary>
	public enum Purpose
	{
		/// <summary>
		/// 練習
		/// </summary>
		Practice = 1,
		/// <summary>
		/// コンプリート目的
		/// </summary>
		Complete = 2,
		/// <summary>
		/// 周回
		/// </summary>
		Lap = 3
	}

	public static class PurposeExtensions
	{
		public static string DisplayName(this Purpose purpose)
		{
			return purpose switch
			{
				Purpose.Practice => "練習",
				Purpose.Complete => "コンプリート目的",
				Purpose.Lap => "周回",
				_ => throw new InvalidEnumArgumentException(nameof(purpose))
			};
		}
	}
}