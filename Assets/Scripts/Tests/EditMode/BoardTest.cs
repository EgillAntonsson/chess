using System.Collections.Generic;
using System.Linq;
using Chess;
using NUnit.Framework;

public class BoardTest
{
	[Test]
	public void Find_valid_moves_on_board()
	{
		var board = new Board(StandardVariant.TileSetup());
		var knight = (TileWithPiece)board.Tiles[1, 0];

		var validMoves = board.FindValidMoves(knight, StandardVariant.ValidMovesByTypeStandard, 1);

		var expectedMoves = new List<Position>
		{
			new(0, 2),
			new(2, 2)
		};
		Assert.That(validMoves.OrderBy(x => x), Is.EqualTo(expectedMoves.OrderBy(x => x)));
	}
	
	[Test]
	public void Find_valid_moves_on_board_for_pawn()
	{
		var board = new Board(StandardVariant.TileSetup());
		var pawn = (TileWithPiece)board.Tiles[0, 1];

		var validMoves = board.FindValidMoves(pawn, StandardVariant.ValidMovesByTypeStandard, 1);

		var expectedMoves = new List<Position>
		{
			new(0, 2),
			new(0, 3)
		};
		Assert.That(validMoves.OrderBy(x => x), Is.EqualTo(expectedMoves.OrderBy(x => x)));
	}
}