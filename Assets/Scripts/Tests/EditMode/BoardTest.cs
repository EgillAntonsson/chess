using System.Collections.Generic;
using Chess;
using NUnit.Framework;

public class BoardTest
{
	[Test]
	public void A_piece_is_updated_on_the_board()
	{
		var piece = new Piece(PieceType.Rook, Color.White);
		var position = new Position(0, 0);
		
		var board = new Board().UpdatePiece(piece, position);
		
		Assert.That(board, Is.EqualTo(new Board().UpdatePiece(piece, position)));
	}
	
	[Test]
	public void Pieces_are_updated_on_the_board_in_one_call()
	{
		var pieces = new List<(Piece, Position)>
		{
			(new Piece(PieceType.Rook, Color.White), new Position(0, 0)),
			(new Piece(PieceType.Knight, Color.White), new Position(1, 0))
		};

		var board = new Board().UpdatePieces(pieces);
		
		Assert.That(board, Is.EqualTo(new Board().UpdatePieces(pieces)));
	}
	
	// [Test]
	// public void Dict_of_pieces_added_to_the_board_at_once()
	// {
	// 	var pieces = new Dictionary<Position, Piece>
	// 	{
	// 		{ new Position(2, 0), new Piece(PieceType.Bishop, Color.White) },
	// 		{ new Position(3, 0), new Piece(PieceType.Queen, Color.White) }
	// 	};
	//
	// 	var board = new Board().UpdatePieces(pieces);
	// 	
	// 	Assert.That(board, Is.EqualTo(new Board().UpdatePieces(pieces)));
	// }
	//
	// [Test]
	// public void Pieces_added_to_the_board_by_differenct_structure_are_equal()
	// {
	// 	var dict = new Dictionary<Position, Piece>
	// 	{
	// 		{ new Position(0, 0), new Piece(PieceType.Rook, Color.White) },
	// 		{ new Position(1, 0), new Piece(PieceType.Knight, Color.White) }
	// 	};
	// 	var list = new List<(Piece, Position)>
	// 	{
	// 		(new Piece(PieceType.Rook, Color.White), new Position(0, 0)),
	// 		(new Piece(PieceType.Knight, Color.White), new Position(1, 0))
	// 	};
	//
	// 	var board = new Board().UpdatePieces(dict);
	// 	
	// 	Assert.That(board, Is.EqualTo(new Board().UpdatePieces(list)));
	// }
}