using System;
using System.Collections.Generic;

namespace Chess
{
	public class StandardVariant : Variant
	{
		static List<Func<ChessBoard, bool>> endConditions = new() { EndCondition.CheckMate };

		static bool CheckMate()
		{
			return false;
		}
		
		
		public StandardVariant() : base(TileSetup.Standard(), endConditions)
		{ }
	}
}
