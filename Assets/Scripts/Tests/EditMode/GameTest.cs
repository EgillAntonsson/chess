

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
		Assert.That(exception.Message, Does.Match("not the turn to move").IgnoreCase);
	}

	[Test]
	public void Not_a_valid_move_position()
	{
		var rules = new Rules();
		var game = new Game(rules);
		var tiles = game.Create();
		var exception = Assert.Throws(Is.TypeOf<ApplicationException>(), delegate { game.MovePiece((TileWithPiece)tiles[0, 1], new Position(0, 4), null); });
		Assert.That(exception.Message, Does.Match("not a valid move position").IgnoreCase);
	}

	[Test(ExpectedResult = false)]
	public bool Should_promotion_occur()
	{
		var rules = new Rules();
		var game = new Game(rules);
		var tiles = game.Create();
		return Game.ShouldPromotionOccur((TileWithPiece)tiles[0, 1], rules);
	}

}