using System;
using System.Collections.Generic;

namespace Chess
{
	public class Variant
	{
		// public virtual VariantType VariantType { get; }
		public virtual string Tiles => @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 P2 P2 P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
P1 P1 P1 P1 P1 P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";

		// public abstract HashSet<EndConditionType> EndConditions { get; }
		
		public virtual HashSet<EndConditionType> EndConditions => new() { EndConditionType.CheckMate };

		public virtual int NumberOfPlayers => 2;
		public virtual int PlayerIdToStart => 1;
		// public virtual IEnumerable<Move> ValidMovesByType(PieceType type, int playerId);
		
		public virtual IEnumerable<Move> ValidMovesByType(PieceType type, int playerId)
		{
			return type switch
			{
				PieceType.Knight => ValidMovesStandard.Knight(),
				PieceType.Pawn => ValidMovesStandard.Pawn(playerId),
				PieceType.Bishop => ValidMovesStandard.Bishop(),
				PieceType.Rook => ValidMovesStandard.Rook(),
				PieceType.Queen => ValidMovesStandard.Queen(),
				PieceType.King => ValidMovesStandard.King(),
				_ => new Move[] { }
			};
		}
		public virtual bool CanCheck => true;
		public virtual PieceType CheckablePieceType => PieceType.King;
		public virtual PieceType CastlingPieceType => PieceType.Rook;

		
	}
}