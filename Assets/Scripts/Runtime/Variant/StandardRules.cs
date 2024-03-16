using System;
using System.Collections.Generic;

namespace Chess
{
	public class StandardRules : Rules
	{
		// public override VariantType VariantType => VariantType.Standard;
		// public override string BoardAtStart => BoardAtStart();

// 		public static string BoardAtStart()
// 		{
// 			return @"
// R2 N2 B2 Q2 K2 B2 N2 R2
// P2 P2 P2 P2 P2 P2 P2 P2
// -- -- -- -- -- -- -- --
// -- -- -- -- -- -- -- --
// -- -- -- -- -- -- -- --
// -- -- -- -- -- -- -- --
// P1 P1 P1 P1 P1 P1 P1 P1
// R1 N1 B1 Q1 K1 B1 N1 R1
// ";
// 		}

		public override PieceType CastlingPieceType => CheckablePieceTypeStandard;
		public static PieceType CaslingPieceTypeStandard => PieceType.Rook;
        
		public override PieceType CheckablePieceType => CheckablePieceTypeStandard;

		public static PieceType CheckablePieceTypeStandard => PieceType.King;

		// public override HashSet<EndConditionType> EndConditions => EndConditionsStandard();
		//
		// public static HashSet<EndConditionType> EndConditionsStandard()
		// {
		// 	return new HashSet<EndConditionType>
		// 	{
		// 		EndConditionType.CheckMate,
		// 	};
		// }

		// public override IEnumerable<Move> ValidMovesByType(PieceType type, int playerId) => ValidMovesByTypeStandard(type, playerId);

		// public static IEnumerable<Move> ValidMovesByTypeStandard(PieceType type, int playerId)
		// {
		// 	return type switch
		// 	{
		// 		PieceType.Knight => ValidMovesStandard.Knight(),
		// 		PieceType.Pawn => ValidMovesStandard.Pawn(playerId),
		// 		PieceType.Bishop => ValidMovesStandard.Bishop(),
		// 		PieceType.Rook => ValidMovesStandard.Rook(),
		// 		PieceType.Queen => ValidMovesStandard.Queen(),
		// 		PieceType.King => ValidMovesStandard.King(),
		// 		_ => new Move[] { }
		// 	};
		// }
	}
}