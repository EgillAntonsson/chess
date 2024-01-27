using System;
using System.Collections.Generic;

namespace Chess
{
	public abstract class Variant
	{
		public abstract VariantType VariantType { get; }
		public abstract string Tiles { get; }
		public abstract HashSet<EndConditionType> EndConditions { get; }
		public virtual int NumberOfPlayers => 2;
		public virtual int PlayerIdToStart => 1;
		public virtual bool CanCheck => true;
		public abstract IEnumerable<Move> ValidMovesByType(PieceType type, int playerId);
	}
}