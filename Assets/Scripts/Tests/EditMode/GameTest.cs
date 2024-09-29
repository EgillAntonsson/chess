using System.Collections.Generic;
using System.Linq;
using Chess;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;

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

	[Test]
	public void UpdatePlayersTurn()
	{
		var players = new List<Player> { new(1), new(2) };

		var playerIdToMove = Game.UpdatePlayerTurn(1, players);
		Assert.That(playerIdToMove, Is.EqualTo(2));
		
		playerIdToMove = Game.UpdatePlayerTurn(2, players);
		Assert.That(playerIdToMove, Is.EqualTo(1));
	}
	
	[Test]
	public void UpdatePlayersTurn_3_player_chess()
	{
		var players = new List<Player> { new(1), new(2), new(3) };

		var playerIdToMove = Game.UpdatePlayerTurn(1, players);
		Assert.That(playerIdToMove, Is.EqualTo(2));
		
		playerIdToMove = Game.UpdatePlayerTurn(2, players);
		Assert.That(playerIdToMove, Is.EqualTo(3));
		
		playerIdToMove = Game.UpdatePlayerTurn(3, players);
		Assert.That(playerIdToMove, Is.EqualTo(1));

		players[1] = new Player(players[1].Id, CheckType.CheckMate);
		
		playerIdToMove = Game.UpdatePlayerTurn(1, players);
		Assert.That(playerIdToMove, Is.EqualTo(3));
	}

	[Test]
	public void CheckEndConditions_results_in_checkmate()
	{
		var rules = new Rules();
		var chessBoard = new ChessBoard(rules);
		var retCreate = chessBoard.Create(rules.BoardAtStart);
		var game = new Game(rules, chessBoard);
		var opponentsInCheck = new List<Player>();
		opponentsInCheck.Add(new Player(2, CheckType.CheckMate));
		var playerIdThatIsMoving = 1;
		
		var ret = game.CheckEndConditions(playerIdThatIsMoving, opponentsInCheck, new Rules().EndConditions, retCreate.Item1);
		
		Assert.That(ret.gameHasEnded, Is.True);
		Assert.That(ret.resultByPlayerId[playerIdThatIsMoving], Is.EqualTo(Result.Win));
	}
	
	[Test]
	public void CheckEndConditions_results_in_stalemate()
	{
		var rules = new Rules();
		var chessBoard = new ChessBoard(rules);
		chessBoard.Create(rules.BoardAtStart);
		var retCreate = chessBoard.Create_ButNotUpdateStartPos(BoardTileString.Stalemate_player_2_can_not_move());
		var game = new Game(rules, chessBoard);
		var opponentsInCheck = new List<Player>();
		opponentsInCheck.Add(new Player(2));
		var playerIdThatIsMoving = 1;
		
		var ret = game.CheckEndConditions(playerIdThatIsMoving, opponentsInCheck, new Rules().EndConditions, retCreate.Item1);
		
		Assert.That(ret.gameHasEnded, Is.True);
		Assert.That(ret.resultByPlayerId[playerIdThatIsMoving], Is.EqualTo(Result.Draw));
	}
}