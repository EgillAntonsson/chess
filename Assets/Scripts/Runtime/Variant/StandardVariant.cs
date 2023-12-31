using System;
using System.Collections.Generic;

namespace Chess
{
	public class StandardVariant : Variant
	{
		public override IEnumerable<Tile> TileSetupSequence => TileSetup();
		public override IEnumerable<Func<ChessBoard, bool>> EndConditions => EndConditionsImpl();
		public override int NumberOfPlayers => 2;

		private static IEnumerable<Func<ChessBoard, bool>> EndConditionsImpl()
		{
			return new List<Func<ChessBoard, bool>> { EndCondition.CheckMate };
		}

		private static IEnumerable<Tile> TileSetup()
		{
			return new List<Tile>
			{
				new (new Position(0, 0), new Piece(PieceType.Rook, 1)),
				new (new Position(1, 0), new Piece(PieceType.Knight, 1)),
				new (new Position(2, 0), new Piece(PieceType.Bishop, 1)),
				new (new Position(3, 0), new Piece(PieceType.Queen, 1)),
				new (new Position(4, 0), new Piece(PieceType.King, 1)),
				new (new Position(5, 0), new Piece(PieceType.Bishop, 1)),
				new (new Position(6, 0), new Piece(PieceType.Knight, 1)),
				new (new Position(7, 0), new Piece(PieceType.Rook, 1)),
				new (new Position(0, 1), new Piece(PieceType.Pawn, 1)),
				new (new Position(1, 1), new Piece(PieceType.Pawn, 1)),
				new (new Position(2, 1), new Piece(PieceType.Pawn, 1)),
				new (new Position(3, 1), new Piece(PieceType.Pawn, 1)),
				new (new Position(4, 1), new Piece(PieceType.Pawn, 1)),
				new (new Position(5, 1), new Piece(PieceType.Pawn, 1)),
				new (new Position(6, 1), new Piece(PieceType.Pawn, 1)),
				new (new Position(7, 1), new Piece(PieceType.Pawn, 1)),
				new (new Position(0, 2)),
				new (new Position(1, 2)),
				new (new Position(2, 2)),
				new (new Position(3, 2)),
				new (new Position(4, 2)),
				new (new Position(5, 2)),
				new (new Position(6, 2)),
				new (new Position(7, 2)),
				new (new Position(0, 3)),
				new (new Position(1, 3)),
				new (new Position(3, 3)),
				new (new Position(3, 3)),
				new (new Position(4, 3)),
				new (new Position(5, 3)),
				new (new Position(6, 3)),
				new (new Position(7, 3)),
				new (new Position(0, 4)),
				new (new Position(1, 4)),
				new (new Position(2, 4)),
				new (new Position(3, 4)),
				new (new Position(4, 4)),
				new (new Position(5, 4)),
				new (new Position(6, 4)),
				new (new Position(7, 4)),
				new (new Position(0, 5)),
				new (new Position(1, 5)),
				new (new Position(2, 5)),
				new (new Position(3, 5)),
				new (new Position(4, 5)),
				new (new Position(5, 5)),
				new (new Position(6, 5)),
				new (new Position(7, 5)),
				new (new Position(0, 6), new Piece(PieceType.Pawn, 2)),
				new (new Position(1, 6), new Piece(PieceType.Pawn, 2)),
				new (new Position(2, 6), new Piece(PieceType.Pawn, 2)),
				new (new Position(3, 6), new Piece(PieceType.Pawn, 2)),
				new (new Position(4, 6), new Piece(PieceType.Pawn, 2)),
				new (new Position(5, 6), new Piece(PieceType.Pawn, 2)),
				new (new Position(6, 6), new Piece(PieceType.Pawn, 2)),
				new (new Position(7, 6), new Piece(PieceType.Pawn, 2)),
				new (new Position(0, 7), new Piece(PieceType.Rook, 2)),
				new (new Position(1, 7), new Piece(PieceType.Knight, 2)),
				new (new Position(2, 7), new Piece(PieceType.Bishop, 2)),
				new (new Position(3, 7), new Piece(PieceType.Queen, 2)),
				new (new Position(4, 7), new Piece(PieceType.King, 2)),
				new (new Position(5, 7), new Piece(PieceType.Bishop, 2)),
				new (new Position(6, 7), new Piece(PieceType.Knight, 2)),
				new (new Position(7, 7), new Piece(PieceType.Rook, 2)),
			};
		}
	}
}