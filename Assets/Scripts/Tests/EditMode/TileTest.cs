using Chess;
using NUnit.Framework;

public class TileTest
{
	[Test]
	// This test is testing the record implementation and thus likely not needed.
	public void Verify_that_tiles_are_equal_by_value()
	{
		// Given that Position is a struct that implements IEquatable properly
		var tileA = new Tile(new Position(0, 0));
		var tileB = new Tile(new Position(0, 0));
		Assert.AreEqual(tileA, tileB);
		Assert.AreNotSame(tileA, tileB);
		Assert.IsTrue(tileA == tileB);
		
		var tileC = new Tile(new Position(1, 0));
		Assert.AreNotEqual(tileC, tileA);
		Assert.AreNotSame(tileC, tileA);
		Assert.IsFalse(tileC == tileA);
	}
	
	[Test]
	public void Verify_that_tile_implement_comparable_based_on_param()
	{
		var posA = new Position(0, 0);
		var posC = new Position(1, 0);
		var tileA = new Tile(posA);
		var tileB = new Tile(posA);
		Assert.That(tileA.CompareTo(tileB), Is.EqualTo(0));
		Assert.That(tileA.CompareTo(tileB), Is.EqualTo(posA.CompareTo(posA)));
		
		var tileC = new Tile(posC);
		Assert.That(tileC.CompareTo(tileA), Is.Not.EqualTo(0));
		Assert.That(tileC.CompareTo(tileA), Is.EqualTo(posC.CompareTo(posA)));
	}
	
	[Test]
	public void Verify_that_tile_with_piece_implement_comparable_based_on_the_params()
	{
		var posA = new Position(0, 0);
		var pieceA = new Piece(PieceType.Bishop, 1);
		var tileA = new TileWithPiece(posA, pieceA);
		var tileB = new TileWithPiece(posA, pieceA);
		Assert.That(tileA.CompareTo(tileB), Is.EqualTo(0));

		var pieceB = new Piece(PieceType.Knight, 1);
		var tileC = new TileWithPiece(posA, pieceB);
		Assert.That(tileC.CompareTo(tileA), Is.Not.EqualTo(0));
		
		var tileD = new TileWithPiece(posA, pieceB, true);
		Assert.That(tileD.CompareTo(tileC), Is.Not.EqualTo(0));
	}
}