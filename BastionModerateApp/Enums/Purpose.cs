using System;
using System.ComponentModel;

namespace BastionModerateApp.Enums
{
	public enum Purpose
	{
		Practice = 1,
		Complete = 2,
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