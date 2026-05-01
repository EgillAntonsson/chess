using System.Collections.Generic;
using System.Threading.Tasks;
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
	public async Task En_passant_capture_is_offered_after_opponent_double_push()
	{
		var rules = new Rules();
		var chessBoard = new ChessBoard(rules);
		var game = new Game(rules, chessBoard);
		var tiles = game.Create();
		// Promotion is not relevant to this test, so we can just return some valid piece type.
		Task<PieceType> noPromotion() => Task.FromResult(PieceType.Queen);

		// P1 double-pushes e-pawn to row 3.
		var (changed1, _, _) = await game.MovePiece((TileWithPiece)tiles[4, 1], new Position(4, 3), noPromotion);
		var p1Pawn = TestUtil.GetMovedTile(changed1, new Position(4, 3));

		// P2 makes a filler move.
		await game.MovePiece((TileWithPiece)tiles[0, 6], new Position(0, 5), noPromotion);

		// P1 advances e-pawn one step to row 4.
		var (changed3, _, _) = await game.MovePiece(p1Pawn, new Position(4, 4), noPromotion);
		var p1PawnAtRow4 = TestUtil.GetMovedTile(changed3, new Position(4, 4));

		// P2 double-pushes d-pawn to row 4, landing adjacent to P1 pawn.
		await game.MovePiece((TileWithPiece)tiles[3, 6], new Position(3, 4), noPromotion);

		// P1 pawn should have en passant capture available.
		var movePositions = game.FindMovePositions(p1PawnAtRow4);
		Assert.That(movePositions, Contains.Item(new Position(3, 5)));
	}

	[Test]
	public async Task Pawn_promotes_to_chosen_piece_type_when_reaching_promotion_row()
	{
		var rules = new Rules();
		var chessBoard = new ChessBoard(rules);
		var game = new Game(rules, chessBoard);
		var tiles = game.Create();
		Task<PieceType> promoteToQueen() => Task.FromResult(PieceType.Queen);

		// P1 a-pawn double-pushes to row three.
		var (zeroThree, _, _) = await game.MovePiece((TileWithPiece)tiles[0, 1], new Position(0, 3), promoteToQueen);
		var p1Pawn = TestUtil.GetMovedTile(zeroThree, new Position(0, 3));

		// P2 filler.
		await game.MovePiece((TileWithPiece)tiles[7, 6], new Position(7, 5), promoteToQueen);

		// P1 pawn advances to row four.
		var (zeroFour, _, _) = await game.MovePiece(p1Pawn, new Position(0, 4), promoteToQueen);
		p1Pawn = TestUtil.GetMovedTile(zeroFour, new Position(0, 4));

		// P2 filler.
		await game.MovePiece((TileWithPiece)tiles[6, 6], new Position(6, 5), promoteToQueen);

		// P1 pawn advances to row five.
		var (zeroFive, _, _) = await game.MovePiece(p1Pawn, new Position(0, 5), promoteToQueen);
		p1Pawn = TestUtil.GetMovedTile(zeroFive, new Position(0, 5));

		// P2 filler.
		await game.MovePiece((TileWithPiece)tiles[5, 6], new Position(5, 5), promoteToQueen);

		// P1 pawn captures diagonally, reaching the second last row.
		var (oneSix, _, _) = await game.MovePiece(p1Pawn, new Position(1, 6), promoteToQueen);
		p1Pawn = TestUtil.GetMovedTile(oneSix, new Position(1, 6));

		// P2 filler.
		await game.MovePiece((TileWithPiece)tiles[4, 6], new Position(4, 5), promoteToQueen);

		// P1 pawn's next move triggers promotion.
		var (zeroSeven, _, _) = await game.MovePiece(p1Pawn, new Position(0, 7), promoteToQueen);
		var promotedTile = TestUtil.GetMovedTile(zeroSeven, new Position(0, 7));

		Assert.That(promotedTile.Piece.Type, Is.EqualTo(PieceType.Queen));
	}

	[Test]
	public async Task CheckEndConditions_results_in_checkmate()
	{
		var rules = new Rules();
		var chessBoard = new ChessBoard(rules);
		chessBoard.Create(rules.BoardAtStart);
		var game = new Game(rules, chessBoard);
		var (tiles, _) = chessBoard.SetBoardState(BoardTileString.Player1_can_CheckMate());

		// Promotion is not relevant to this test, so we can just return some valid piece type.
		Task<PieceType> noPromotion() => Task.FromResult(PieceType.Queen);

		Assert.That(game.GameHasEnded, Is.False);

		var (_, _, playersEndResult) = await game.MovePiece((TileWithPiece)tiles[3, 0], new Position(7, 4), noPromotion);

		Assert.That(game.GameHasEnded, Is.True);
		Assert.That(playersEndResult[game.PlayerIdToMove], Is.EqualTo(Result.Win));
	}

	[Test]
	public void CheckEndConditions_results_in_stalemate()
	{
		var rules = new Rules();
		var chessBoard = new ChessBoard(rules);
		chessBoard.Create(rules.BoardAtStart);
		var (tiles, _) = chessBoard.SetBoardState(BoardTileString.Stalemate_player_2_can_not_move());
		var game = new Game(rules, chessBoard);
		var opponentsInCheck = new List<Player>
		{
			new(2)
		};
		var playerIdThatIsMoving = 1;

		var (gameHasEnded, resultByPlayerId) = game.CheckEndConditions(playerIdThatIsMoving, opponentsInCheck, rules.EndConditions, tiles);

		Assert.That(gameHasEnded, Is.True);
		Assert.That(resultByPlayerId[playerIdThatIsMoving], Is.EqualTo(Result.Draw));
	}
}
