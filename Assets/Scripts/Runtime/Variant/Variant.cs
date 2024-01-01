using System;
using System.Collections.Generic;

namespace Chess
{
	public abstract class Variant
	{
		public abstract VariantType VariantType { get;  }
		public abstract IEnumerable<Tile> TileSetupSequence { get; }
		public abstract IEnumerable<Func<Board, bool>> EndConditions { get; }
		public virtual int NumberOfPlayers => 2;
		public virtual int PlayerIdToStart => 1;

		public abstract IEnumerable<Position> ValidMovesByType(PieceType type);
	}
}
