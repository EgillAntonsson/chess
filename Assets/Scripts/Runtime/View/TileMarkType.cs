using System;

namespace Chess.View
{
	public enum TileMarkType
	{
		Normal = 0,
		Selected = 1,
		ValidMove = 2,
		Check = 3
	}
	
	public static class TileMarkTypeUtil {
		public static TileMarkType ConvertFromCheckType(CheckType checkType)
		{
			return checkType switch
			{
				CheckType.NoCheck => TileMarkType.Normal,
				CheckType.Check => TileMarkType.Check,
				CheckType.CheckMate => TileMarkType.Check,
				_ => TileMarkType.Normal
			};
		}
	}
}