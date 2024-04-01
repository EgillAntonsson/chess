using System.Collections.Generic;

namespace Chess
{
	public class Rules
	{
		public virtual string BoardAtStart => @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 P2 P2 P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
P1 P1 P1 P1 P1 P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
		
		public virtual HashSet<EndConditionType> EndConditions => new() { EndConditionType.CheckMate };

		public virtual int NumberOfPlayers => 2;
		public virtual int PlayerIdToStart => 1;
		
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
		public virtual PieceType CheckablePieceType => PieceType.King;
		public virtual PieceType CastlingPieceType => PieceType.Rook;
		public virtual PieceType InPassingPieceType => PieceType.Pawn;
		

		
	}
}