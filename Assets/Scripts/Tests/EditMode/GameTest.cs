using Chess;
using NUnit.Framework;

public class GameTest
{
	[TestCase(0,1, PieceType.Pawn,  1, 1, ExpectedResult = false)]
	[TestCase(3, 0, PieceType.Queen, 1, 0, ExpectedResult = false)]
	[TestCase(0, 1, PieceType.Pawn, 1, 6, ExpectedResult = true)]
	[TestCase(0, 1, PieceType.Pawn, 1, 6, ExpectedResult = true)]
	[TestCase(0, 6, PieceType.Pawn, 2, 6, ExpectedResult = false)]
	[TestCase(0, 6, PieceType.Pawn, 2, 1, ExpectedResult = true)]
	public bool Should_promotion_occur(int column, int row, PieceType type, int playerId, int currentRow)
	{
		var twp = new TileWithPiece(new Position(column, row), new Piece(type, playerId)) { Position = new Position(column, currentRow) };
		return Game.ShouldPromotionOccur(twp, new Rules());
	}
}