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