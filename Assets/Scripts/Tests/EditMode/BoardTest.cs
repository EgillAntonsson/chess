using System;
using System.Collections.Generic;
using System.Linq;
using Chess;
using NUnit.Framework;

public class BoardTest
{
	private static (Tile[,] boardTiles, Dictionary<Position, TileWithPiece> tileByStartPos, Dictionary<int, IEnumerable<TileWithPiece>> tilesByPlayer)
		CreateBoard(string boardTiles, Rules rules)
	{
		return Board.Create(boardTiles, rules.CheckablePieceType, rules.CastlingPieceType);
	}
	
	[Test]
	public void Create_board()
	{
		var rules = new Rules();
		var (tiles, tileByStartPos, _) = CreateBoard(rules.BoardAtStart, rules);

		Assert.That(tiles.Length, Is.EqualTo(64));
		var pos = new Position(0, 0);
		TileWithPiece twp = new TileWithCastlingPiece(pos, new Piece(PieceType.Rook, 1));
		Assert.That(tiles[pos.Column, pos.Row], Is.EqualTo(twp));
		Assert.That(tileByStartPos[pos], Is.EqualTo(twp));

		pos = new Position(0, 1);
		twp = new TileWithPiece(pos, new Piece(PieceType.Pawn, 1));
		Assert.That(tiles[pos.Column, pos.Row], Is.EqualTo(twp));
		Assert.That(tileByStartPos[pos], Is.EqualTo(twp));

		pos = new Position(4, 0);
		twp = new TileWithCheckablePiece(pos, new Piece(PieceType.King, 1));
		Assert.That(tileByStartPos[pos], Is.EqualTo(twp));
	}

	public static IEnumerable<TestCaseData> FindMoveCases()
	{
		var pos = new Position(1, 0);
		var playerIdToMove = 1;
		var rules = new Rules();
		Func<string> currentBoardFunc = () => rules.BoardAtStart;
		IEnumerable<Position> expectedMoves = new Position[] { new(0, 2), new(2, 2) };
		yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.Knight} on {pos} when {currentBoardFunc.Method.Name}");

		pos = new Position(0, 1);
		currentBoardFunc = () => rules.BoardAtStart;
		expectedMoves = new Position[] { new(0, 2), new(0, 3) };
		yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.Pawn} on {pos} when {currentBoardFunc.Method.Name}");

		yield return new TestCaseData(pos, BoardTileString.Notation_1_e4_c5, playerIdToMove, expectedMoves).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.Pawn} on {pos} when {nameof(BoardTileString.Notation_1_e4_c5)}");

		pos = new Position(4, 3);
		expectedMoves = new Position[] { new(4, 4) };
		yield return new TestCaseData(pos, BoardTileString.Notation_1_e4_c5, playerIdToMove, expectedMoves).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.Pawn} on {pos} when {nameof(BoardTileString.Notation_1_e4_c5)}");

		pos = new Position(4, 3);
		currentBoardFunc = BoardTileString.Notation_1_e4_e5;
		expectedMoves = Enumerable.Empty<Position>();
		yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.Pawn} on {pos} when {currentBoardFunc.Method.Name}");

		pos = new Position(4, 0);
		currentBoardFunc = () => rules.BoardAtStart;
		yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.King} on {pos} when {currentBoardFunc.Method.Name}");

		pos = new Position(5, 0);
		currentBoardFunc = () => rules.BoardAtStart;
		yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.Bishop} on {pos} when {currentBoardFunc.Method.Name}");

		pos = new Position(0, 0);
		currentBoardFunc = () => rules.BoardAtStart;
		yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.Rook} on {pos} when {currentBoardFunc.Method.Name}");

		pos = new Position(3, 0);
		currentBoardFunc = () => rules.BoardAtStart;
		yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.Queen} on {pos} when {currentBoardFunc.Method.Name}");

		playerIdToMove = 2;
		pos = new Position(3, 7);
		currentBoardFunc = BoardTileString.Quickest_Win__Player2_To_Move;
		expectedMoves = new Position[]
		{
			new(4, 6),
			new(5, 5),
			new(6, 4),
			new(7, 3)
		};
		yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.Queen} on {pos} when {currentBoardFunc.Method.Name}");

		playerIdToMove = 1;
		pos = new Position(4, 0);
		expectedMoves = new Position[]
		{
			new(4, 1),
		};
		yield return new TestCaseData(pos, BoardTileString.Notation_1_e4_c5, playerIdToMove, expectedMoves).SetName(
			$"{nameof(Find_moves_on_board_for)} {PieceType.King} on {pos} when {nameof(BoardTileString.Notation_1_e4_c5)}");
	}

	[TestCaseSource(nameof(FindMoveCases))]
	public void Find_moves_on_board_for(Position piecePos, string tilesAtCurrent, int playerIdToMove, IEnumerable<Position> expectedMoves)
	{
		var rules = new Rules();
		var (_, pieceTypeByStartPositions, _) = CreateBoard(rules.BoardAtStart, rules);
		var (tiles, _, _) = CreateBoard(tilesAtCurrent, rules);
		var twp = (TileWithPiece)Board.GetTile(piecePos, tiles);

		var validMoves = Board.FindMovePositions(twp,
			rules.MoveDefinitionByType,
			playerIdToMove,
			tiles,
			pieceTypeByStartPositions,
			MoveCaptureFlag.Move | MoveCaptureFlag.Capture);

		TestUtil.AssertArraysAreEqual(validMoves, expectedMoves);
	}

	[Test]
	public void Is_not_in_check_at_start()
	{
		var rules = new Rules();
		var (tiles, tilesByStartPos, tilesByPlayer) = CreateBoard(rules.BoardAtStart, rules);
		var p = new Position(4, 0);
		var kingTile = new TileWithPiece(p, new Piece(PieceType.King, 1));
		const int playerId = 1;

		var opponentCheck = Board.IsInCheck(kingTile, rules.MoveDefinitionByType, ChessBoard.GetOpponentTiles(tilesByPlayer, playerId), tiles, tilesByStartPos, tilesByPlayer[playerId]);

		Assert.That(opponentCheck, Is.EqualTo(CheckType.NoCheck));
	}

	public static IEnumerable<TestCaseData> IsInCheckCases()
	{
		var playerId = 1;
		var p = new Position(4, 0);
		var kingTile = new TileWithPiece(p, new Piece(PieceType.King, playerId));
		Func<string> currentBoardFunc = BoardTileString.Check_but_piece_can_defend;
		yield return new TestCaseData(currentBoardFunc(), playerId, kingTile).SetName(
			$"{nameof(Is_in_check_returns_check)} when {currentBoardFunc.Method.Name}");

		playerId = 1;
		kingTile = new TileWithPiece(p, new Piece(PieceType.King, playerId));
		currentBoardFunc = BoardTileString.Check_but_king_can_move_but_not_castle;
		yield return new TestCaseData(currentBoardFunc(), playerId, kingTile).SetName(
			$"{nameof(Is_in_check_returns_check)} when {currentBoardFunc.Method.Name}");
	}

	[TestCaseSource(nameof(IsInCheckCases))]
	public void Is_in_check_returns_check(string tilesAtCurrent, int playerId, TileWithPiece kingTile)
	{
		var rules = new Rules();
		var (tiles, tilesByStartPos, tilesByPlayer) = CreateBoard(BoardTileString.Check_but_king_can_move_but_not_castle(), rules);

		var opponentCheck = Board.IsInCheck(kingTile, new Rules().MoveDefinitionByType, ChessBoard.GetOpponentTiles(tilesByPlayer, playerId), tiles, tilesByStartPos, tilesByPlayer[playerId]);

		Assert.That(opponentCheck, Is.EqualTo(CheckType.Check));
	}

	[Test]
	public void Is_check_mate()
	{
		var rules = new Rules();
		var (_, tilesByStartPos, _) = CreateBoard(rules.BoardAtStart, rules);
		var (tiles, _, tilesByPlayer) = CreateBoard(BoardTileString.Player1_CheckMate(), rules);
		var p = new Position(4, 0);
		var kingTile = new TileWithCheckablePiece(p, new Piece(PieceType.King, 1));
		const int playerId = 1;

		var opponentCheck = Board.IsInCheck(kingTile, rules.MoveDefinitionByType, ChessBoard.GetOpponentTiles(tilesByPlayer, playerId), tiles, tilesByStartPos, tilesByPlayer[playerId]);

		Assert.That(opponentCheck, Is.EqualTo(CheckType.CheckMate));
	}

	public static IEnumerable<TestCaseData> MovePieceCases()
	{
		const int playerId = 1;
		var beforePos = new Position(4, 1);
		var afterPos = new Position(4, 3);
		yield return new TestCaseData(beforePos, afterPos, BoardTileString.Notation_1_e4, playerId).SetName(
			$"{nameof(Move_piece_on_board)} Pawn from {beforePos} to {afterPos} with board after move being {nameof(BoardTileString.Notation_1_e4)}");
	}

	[TestCaseSource(nameof(MovePieceCases))]
	public void Move_piece_on_board(Position beforePos, Position afterPos, string expectedTilesAfterMove, int playerId)
	{
		var rules = new Rules();
		var (tilesBefore, _, tilesByPlayerBefore) = CreateBoard(rules.BoardAtStart, rules);
		var twp = (TileWithPiece)Board.GetTile(beforePos, tilesBefore);

		var (beforeMoveTile, afterMoveTile, tilesAfterMove, tilesByPlayerAfterMove) = Board.MovePiece(twp, afterPos, tilesBefore, tilesByPlayerBefore[playerId]);
		Assert.That(beforeMoveTile, Is.EqualTo(new Tile(beforePos)));
		Assert.That(afterMoveTile, Is.EqualTo(twp with { Position = afterPos, HasMoved = true, FirstMove = true}));
		var (expTilesAfterMove, _, expTilesByPlayers) = CreateBoard(expectedTilesAfterMove, rules);
		
		// TODO: revist and evaluate commented out assert below.
		// Assert.That(tilesAfterMove, Is.EqualTo(expTilesAfterMove));
		var expTileByPlayer = expTilesByPlayers[playerId];
		// Assert.That(tilesByPlayerAfterMove, Is.EqualTo(expTileByPlayer));
	}

	[Test]
	public void PromotePiece()
	{
		var rules = new Rules();
		var (tiles, _, tilesByPlayerBefore) = CreateBoard(BoardTileString.One_move_before_promotion(), rules);
		var tuple = Board.PromotePiece((TileWithPiece)tiles[0, 6], PieceType.Queen, tiles);
		var expectedTwp = new TileWithPiece(new Position(0, 6), new Piece(PieceType.Queen, 1));
		Assert.That(tuple.promotedTile, Is.EqualTo(expectedTwp));
		Assert.That(tuple.tiles[0,6], Is.EqualTo(expectedTwp));
	}
}