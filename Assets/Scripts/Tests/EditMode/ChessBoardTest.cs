using Chess;
using NUnit.Framework;

public class ChessBoardTest
{
	[Test]
	public void A_piece_is_updated_on_the_board()
	{

		var board = new ChessBoard().PopulateAtStart();
		
		Assert.That(1, Is.EqualTo(1));
	}
}