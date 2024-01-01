using System.Collections.Generic;
using Chess;
using NUnit.Framework;

public class BoardTest
{
	[Test]
	public void A_piece_is_updated_on_the_board()
	{

		var tile = new Tile(new Position(0, 0), new Piece(PieceType.Rook, 1));
		var tileSequence = new List<Tile> { tile };
		var board = new Board(tileSequence);
		var anotherTile = new Tile(new Position(0, 0), new Piece(PieceType.Knight, 2)); 
		
			var retBoard = board.UpdateTile(anotherTile);

			Assert.That(retBoard, Is.EqualTo(new Board(new List<Tile> { anotherTile })));
	}
	
	[Test]
	public void Pieces_are_updated_on_the_board_in_one_call()
	{
		var tile = new Tile(new Position(0, 0), new Piece(PieceType.Rook, 1));
		var tileSequence = new List<Tile> { tile };
		var board = new Board(tileSequence);
		var anotherTile = new Tile(new Position(0, 0), new Piece(PieceType.Knight, 2)); 
		
		var anotherList = new List<Tile> { anotherTile };
		var retBoard = board.UpdateTiles(anotherList);

		Assert.That(retBoard, Is.EqualTo(new Board(anotherList)));
	}
}