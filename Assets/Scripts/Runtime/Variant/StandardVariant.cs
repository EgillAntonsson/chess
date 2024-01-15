using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class StandardVariant : Variant
	{
		public override VariantType VariantType => VariantType.Standard;
		public override string Tiles => TilesStandard();
		public override IEnumerable<Func<Board, bool>> EndConditions => EndConditionsStandard();

		public static IEnumerable<Func<Board, bool>> EndConditionsStandard()
		{
			return new List<Func<Board, bool>> { Chess.EndConditions.CheckMate };
		}

		public override IEnumerable<Move> ValidMovesByType(PieceType type, int playerId) => ValidMovesByTypeStandard(type, playerId);

		public static IEnumerable<Move> ValidMovesByTypeStandard(PieceType type, int playerId)
		{
			return type switch
			{
				PieceType.Knight => ValidMovesStandard.Knight(),
				PieceType.Pawn => ValidMovesStandard.Pawn(playerId),
				PieceType.Bishop => ValidMovesStandard.Knight(),
				PieceType.Rook => ValidMovesStandard.Knight(),
				PieceType.Queen => ValidMovesStandard.Knight(),
				PieceType.King => ValidMovesStandard.Knight(),
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, $"PieceType {type} not handled.")
			};
		}

		public static string TilesStandard()
		{
			return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 P2 P2 P2 P2 P2
__ __ __ __ __ __ __ __
__ __ __ __ __ __ __ __
__ __ __ __ __ __ __ __
__ __ __ __ __ __ __ __
P1 P1 P1 P1 P1 P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
		}
		
	}
}