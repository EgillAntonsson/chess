

using System;
using Chess;
using NUnit.Framework;

public class GameTest
{
	[Test]
	public void Not_players_turn_to_move()
	{
		var rules = new Rules();
		var game = new Game(rules);
		var tiles = game.Create();
		var exception = Assert.Throws(Is.TypeOf<ApplicationException>(), delegate
		{
			game.MovePiece((TileWithPiece)tiles[0, 7], new Position(0, 6), null);
		});
		Assert.That(exception.Message, Does.Match("not player's turn to move").IgnoreCase);
	}
}