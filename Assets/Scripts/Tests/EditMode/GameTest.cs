

using System;
using Chess;
using NUnit.Framework;

public class GameTest
{
	// TODO: revisit, now does not work because of async additions.
	// [Test]
	// public void Not_players_turn_to_move()
	// {
	// 	var rules = new Rules();
	// 	var game = new Game(rules);
	// 	var tiles = game.Create();
	// 	var exception = Assert.Throws(Is.TypeOf<ApplicationException>(), delegate
	// 	{
	// 		_ = game.MovePiece((TileWithPiece)tiles[0, 7], new Position(0, 6), null);
	// 	});
	// 	Assert.That(exception.Message, Does.Match("not the turn to move").IgnoreCase);
	// }
	//
	// [Test]
	// public void Not_a_valid_move_position()
	// {
	// 	var rules = new Rules();
	// 	var game = new Game(rules);
	// 	var tiles = game.Create();
	// 	var exception = Assert.Throws(Is.TypeOf<ApplicationException>(),
	// 		delegate
	// 		{
	// 			_ = game.MovePiece((TileWithPiece)tiles[0, 1], new Position(0, 4), null);
	// 		});
	// 	Assert.That(exception.Message, Does.Match("not a valid move position").IgnoreCase);
	// }
	
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