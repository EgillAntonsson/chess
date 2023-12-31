using System.Collections.Generic;
using Chess;
using NUnit.Framework;

public class BoardTest
{
	[Test]
	public void A_piece_is_updated_on_the_board()
	{

		var tile = new Tile(new Position(0, 0), new Piece(PieceType.Rook, Color.White));
		var tileSequence = new List<Tile> { tile };
		var board = new Board(tileSequence);
		var anotherTile = new Tile(new Position(0, 0), new Piece(PieceType.Knight, Color.White)); 
		
			var retBoard = board.UpdatePiece(anotherTile);

			Assert.That(retBoard, Is.EqualTo(new Board(new List<Tile> { anotherTile })));
	}
	
	[Test]
	public void Pieces_are_updated_on_the_board_in_one_call()
	{
		var tile = new Tile(new Position(0, 0), new Piece(PieceType.Rook, Color.White));
		var tileSequence = new List<Tile> { tile };
		var board = new Board(tileSequence);
		var anotherTile = new Tile(new Position(0, 0), new Piece(PieceType.Knight, Color.White)); 
		
		var anotherList = new List<Tile> { anotherTile };
		var retBoard = board.UpdatePieces(anotherList);

		Assert.That(retBoard, Is.EqualTo(new Board(anotherList)));
	}
}