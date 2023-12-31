using System;
using System.Collections.Generic;

namespace Chess
{
	public abstract class Variant
	{
		public abstract IEnumerable<Tile> TileSetupSequence { get; }
		public abstract IEnumerable<Func<ChessBoard, bool>> EndConditions { get; }
		public abstract int NumberOfPlayers { get; }

	}
}
