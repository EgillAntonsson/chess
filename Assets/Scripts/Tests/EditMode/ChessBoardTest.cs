using Chess;
using NUnit.Framework;

public class ChessBoardTest
{
	[Test]
	public void board_is_constructed()
	{
		var board = new ChessBoard(VariantFactory.Create(VariantType.Standard));
		Assert.That(1, Is.EqualTo(1));
	}
}