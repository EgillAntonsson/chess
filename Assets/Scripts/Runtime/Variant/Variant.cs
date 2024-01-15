using System;
using System.Collections.Generic;

namespace Chess
{
	public abstract class Variant
	{
		public abstract VariantType VariantType { get; }
		public abstract string Tiles { get; }
		public abstract IEnumerable<Func<Board, bool>> EndConditions { get; }
		public virtual int NumberOfPlayers => 2;
		public virtual int PlayerIdToStart => 1;
		public abstract IEnumerable<Move> ValidMovesByType(PieceType type, int playerId);
	}
}