using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class StandardVariant : Variant
	{
		public override VariantType VariantType => VariantType.Standard;
		public override IEnumerable<IEnumerable<Tile>> TileSetupSequence => TileSetup();
		public override IEnumerable<Func<Board, bool>> EndConditions => EndConditionsStandard();

		public static IEnumerable<Func<Board, bool>> EndConditionsStandard()
		{
			return new List<Func<Board, bool>> { Chess.EndConditions.CheckMate };
		}

		public override IEnumerable<Position> ValidMovesByType(PieceType type) => ValidMovesByTypeStandard(type);

		public static IEnumerable<Position> ValidMovesByTypeStandard(PieceType type)
		{
			return type switch
			{
				PieceType.Knight => ValidMoves.Knight(),
				PieceType.Pawn => new Position[] { },
				PieceType.Bishop => new Position[] { },
				PieceType.Rook => new Position[] { },
				PieceType.Queen => new Position[] { },
				PieceType.King => new Position[] { },
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, ($"PieceType {type} not handled."))
			};
		}

		public static IEnumerable<IEnumerable<Tile>> TileSetup()
		{
			var player1 = 1;
			var player2 = 2;
			var t = new List<IEnumerable<Tile>>();
			t.Add(
		CreatePieceRow(0, player1,
				new[]
				{
					PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook
				}));
			t.Add(CreatePieceRow(1, player1,
				new[]
				{
					PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn
				}));
			t.Add(CreateEmptyRow(2, 8));
			t.Add(CreateEmptyRow(3, 8));
			t.Add(CreateEmptyRow(4, 8));
			t.Add(CreateEmptyRow(5, 8));
			t.Add(CreatePieceRow(6, player2,
				new[]
				{
					PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn
				}));
			t.Add(CreatePieceRow(7, player2,
				new[]
				{
					PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook
				}));

			return t;
		}

	}
}