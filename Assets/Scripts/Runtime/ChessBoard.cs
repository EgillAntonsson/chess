using System.Collections.Generic;

namespace Chess
{
	public class ChessBoard
	{
		public const int BoardSize = 8;

		private Board board;
		
		public ChessBoard()
		{
			board = new Board(BoardSize);
		}
		
		public ChessBoard PopulateAtStart()
		{
			var pieces = new List<(Piece, Position)>
			{
				(new Piece(PieceType.Rook, Color.White), new Position(0, 0)),
				(new Piece(PieceType.Knight, Color.White), new Position(1, 0)),
				(new Piece(PieceType.Bishop, Color.White), new Position(2, 0)),
				(new Piece(PieceType.Queen, Color.White), new Position(3, 0)),
				(new Piece(PieceType.King, Color.White), new Position(4, 0)),
				(new Piece(PieceType.Bishop, Color.White), new Position(5, 0)),
				(new Piece(PieceType.Knight, Color.White), new Position(6, 0)),
				(new Piece(PieceType.Rook, Color.White), new Position(7, 0)),
				(new Piece(PieceType.Pawn, Color.White), new Position(0, 1)),
				(new Piece(PieceType.Pawn, Color.White), new Position(1, 1)),
				(new Piece(PieceType.Pawn, Color.White), new Position(2, 1)),
				(new Piece(PieceType.Pawn, Color.White), new Position(3, 1)),
				(new Piece(PieceType.Pawn, Color.White), new Position(4, 1)),
				(new Piece(PieceType.Pawn, Color.White), new Position(5, 1)),
				(new Piece(PieceType.Pawn, Color.White), new Position(6, 1)),
				(new Piece(PieceType.Pawn, Color.White), new Position(7, 1)),
				
				(new Piece(PieceType.Rook, Color.Black), new Position(0, 7)),
				(new Piece(PieceType.Knight, Color.Black), new Position(1, 7)),
				(new Piece(PieceType.Bishop, Color.Black), new Position(2, 7)),
				(new Piece(PieceType.Queen, Color.Black), new Position(3, 7)),
				(new Piece(PieceType.King, Color.Black), new Position(4, 7)),
				(new Piece(PieceType.Bishop, Color.Black), new Position(5, 7)),
				(new Piece(PieceType.Knight, Color.Black), new Position(6, 7)),
				(new Piece(PieceType.Rook, Color.Black), new Position(7, 7)),
				(new Piece(PieceType.Pawn, Color.Black), new Position(1, 6)),
				(new Piece(PieceType.Pawn, Color.Black), new Position(2, 6)),
				(new Piece(PieceType.Pawn, Color.Black), new Position(3, 6)),
				(new Piece(PieceType.Pawn, Color.Black), new Position(4, 6)),
				(new Piece(PieceType.Pawn, Color.Black), new Position(5, 6)),
				(new Piece(PieceType.Pawn, Color.Black), new Position(6, 6)),
				(new Piece(PieceType.Pawn, Color.Black), new Position(7, 6)),
			};
			board.UpdatePieces(pieces);
			return this;
		}
	}
}
