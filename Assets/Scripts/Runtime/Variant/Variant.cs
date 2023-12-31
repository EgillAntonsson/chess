using System;
using System.Collections.Generic;

namespace Chess
{
	public class Variant
	{
		public IEnumerable<Tile> TileSetupSequence { get; }
		public IEnumerable<Func<ChessBoard, bool>> EndConditions { get; }
		
		public Variant(IEnumerable<Tile> tileSetupSequence, IEnumerable<Func<ChessBoard, bool>> endConditions)
		{
			TileSetupSequence = tileSetupSequence;
			EndConditions = endConditions;
		}
		
	}
}
