using Chess;
using NUnit.Framework;

public class TileTest
{
	[Test]
	public void Verify_that_tiles_are_equal_by_value()
	{
		var tileA = new TileWithPiece(new Position(0, 0), new Piece(PieceType.Knight, 1));
		var tileB = new TileWithPiece(new Position(0, 0), new Piece(PieceType.Knight, 1));
		Assert.AreEqual(tileA, tileB);
		Assert.AreNotSame(tileA,tileB);
		Assert.IsTrue(tileA == tileB);
	}
}