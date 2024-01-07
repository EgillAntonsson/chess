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
		var knightB1 = (TileWithPiece)board.Tiles[1, 0];

		var validMoves = board.FindValidMoves(knightB1, StandardVariant.ValidMovesByTypeStandard, 1);

		var expectedMoves = new List<Position>
		{
			new(0, 2),
			new(2, 2)
		};
		Assert.That(validMoves.OrderBy(x => x), Is.EqualTo(expectedMoves.OrderBy(x => x)));
	}
}