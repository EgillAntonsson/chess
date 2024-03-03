using System;
using System.Collections.Generic;
using System.Linq;
using Chess;
using NUnit.Framework;

public class BoardTest
{
	[Test]
	public void Create_board()
	{
		var (tiles, tileByStartPos, _) = Board.Create(StandardVariant.BoardAtStart());

		Assert.That(tiles.Length, Is.EqualTo(64));
		Assert.That(tiles[0, 0], Is.EqualTo(new TileWithPiece(new Position(0, 0), new Piece(PieceType.Rook, 1))));

		Assert.That(tileByStartPos[new Position(0, 0)], Is.EqualTo(new TileWithPiece(new Position(0, 0), new Piece(PieceType.Rook, 1))));
		Assert.That(tileByStartPos[new Position(0, 1)], Is.EqualTo(new TileWithPiece(new Position(0, 1), new Piece(PieceType.Pawn, 1))));
	}

	public static IEnumerable<TestCaseData> FoundMoveCases
	{
		get
		{
			var pos = new Position(1, 0);
			var playerIdToMove = 1;
			Func<string> currentBoardFunc = StandardVariant.BoardAtStart;
			IEnumerable<Position> expectedMoves = new Position[] { new(0, 2), new(2, 2) };
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_moves_on_board_for)} {PieceType.Knight} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(0, 1);
			expectedMoves = new Position[] { new(0, 2), new(0, 3) };
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_moves_on_board_for)} {PieceType.Pawn} on {pos} when {currentBoardFunc.Method.Name}");

			currentBoardFunc = BoardTileString.Notation_1_e4_c5;
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_moves_on_board_for)} {PieceType.Pawn} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(4, 3);
			expectedMoves = new Position[] { new(4, 4) };
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_moves_on_board_for)} {PieceType.Pawn} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(4, 3);
			currentBoardFunc = BoardTileString.Notation_1_e4_e5;
			expectedMoves = Enumerable.Empty<Position>();
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_moves_on_board_for)} {PieceType.Pawn} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(4, 0);
			currentBoardFunc = StandardVariant.BoardAtStart;
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
				$"{nameof(Find_moves_on_board_for)} {PieceType.King} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(5, 0);
			currentBoardFunc = StandardVariant.BoardAtStart;
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
				$"{nameof(Find_moves_on_board_for)} {PieceType.Bishop} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(0, 0);
			currentBoardFunc = StandardVariant.BoardAtStart;
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
				$"{nameof(Find_moves_on_board_for)} {PieceType.Rook} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(3, 0);
			currentBoardFunc = StandardVariant.BoardAtStart;
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
			currentBoardFunc = BoardTileString.Notation_1_e4_c5;
			expectedMoves = new Position[]
			{
				new(4, 1),
			};
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_moves_on_board_for)} {PieceType.King} on {pos} when {currentBoardFunc.Method.Name}");
			
			playerIdToMove = 1;
			pos = new Position(4, 0);
			currentBoardFunc = BoardTileString.Player1_can_castle_king_side;
			expectedMoves = new Position[]
			{
					new(4, 1),
					new(5, 0),
					new(6,0)
			};
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
					$"{nameof(Find_moves_on_board_for)} {PieceType.King} on {pos} when {currentBoardFunc.Method.Name}");
		}
	}

	[TestCaseSource(nameof(FoundMoveCases))]
	public void Find_moves_on_board_for(Position piecePos, string tilesAtCurrent, int playerIdToMove, IEnumerable<Position> expectedMoves)
	{
		var (_, pieceTypeByStartPositions, _) = Board.Create(StandardVariant.BoardAtStart());
		var (tiles, _, tilesByPlayer) = Board.ConvertBoardStringToTiles(tilesAtCurrent);
		var twp = (TileWithPiece)Board.GetTile(piecePos, tiles);
		
		var validMoves = Board.FindMovesPos(twp,
			StandardVariant.ValidMovesByTypeStandard,
			playerIdToMove,
			tiles,
			pieceTypeByStartPositions,
			MoveCaptureFlag.Move | MoveCaptureFlag.Capture);

		TestUtil.AssertArraysAreEqual(validMoves, expectedMoves);
	}

	[Test]
	public void Is_not_in_check_at_start()
	{
		var (tiles, tilesByStartPos, tilesByPlayer) = Board.Create(StandardVariant.BoardAtStart());
		var kingTile = new TileWithPiece(new Position(4, 0), new Piece(PieceType.King, 1));
		const int playerId = 1;
		
		var opponentCheck = Board.IsInCheck(kingTile, StandardVariant.ValidMovesByTypeStandard, ChessBoard.GetOpponentTiles(tilesByPlayer, playerId), tiles, tilesByStartPos, tilesByPlayer[playerId]);

		Assert.That(opponentCheck, Is.EqualTo(CheckType.NoCheck));
	}
	
	public static IEnumerable<TestCaseData> IsInCheckCases
	{
		get
		{
			var playerId = 1;
			var kingTile = new TileWithPiece(new Position(4, 0), new Piece(PieceType.King, playerId));
			Func<string> currentBoardFunc = BoardTileString.Check_but_piece_can_defend;
			yield return new TestCaseData(currentBoardFunc(), playerId, kingTile).SetName(
					$"{nameof(Is_in_check_returns_check)} when {currentBoardFunc.Method.Name}");
			
			playerId = 1;
			kingTile = new TileWithPiece(new Position(4, 0), new Piece(PieceType.King, playerId));
			currentBoardFunc = BoardTileString.Check_but_king_can_move;
			yield return new TestCaseData(currentBoardFunc(), playerId, kingTile).SetName(
					$"{nameof(Is_in_check_returns_check)} when {currentBoardFunc.Method.Name}");
		}
	}

	[TestCaseSource(nameof(IsInCheckCases))]
	public void Is_in_check_returns_check(string tilesAtCurrent, int playerId, TileWithPiece kingTile)
	{
		var (tiles, tilesByStartPos, tilesByPlayer) = Board.Create(BoardTileString.Check_but_king_can_move());
		
		var opponentCheck = Board.IsInCheck(kingTile, StandardVariant.ValidMovesByTypeStandard, ChessBoard.GetOpponentTiles(tilesByPlayer, playerId), tiles, tilesByStartPos, tilesByPlayer[playerId]);

		Assert.That(opponentCheck, Is.EqualTo(CheckType.Check));
	}

	[Test]
	public void Is_check_mate()
	{
		Board.Create(StandardVariant.BoardAtStart());
		var (tiles, tilesByStartPos, tilesByPlayer) = Board.ConvertBoardStringToTiles(BoardTileString.Player1_CheckMate());
		var kingTile = new TileWithPiece(new Position(4, 0), new Piece(PieceType.King, 1));
		const int playerId = 1;
		
		var opponentCheck = Board.IsInCheck(kingTile, StandardVariant.ValidMovesByTypeStandard, ChessBoard.GetOpponentTiles(tilesByPlayer, playerId), tiles, tilesByStartPos, tilesByPlayer[playerId]);

		Assert.That(opponentCheck, Is.EqualTo(CheckType.CheckMate));
	}

	public static IEnumerable<TestCaseData> MovePieceCases
	{
		get
		{
			const int playerId = 1;
			var beforePos = new Position(4, 1);
			var afterPos = new Position(4, 3);
			Func<string> expectedBoardAfterMoveFunc = BoardTileString.Notation_1_e4;
			yield return new TestCaseData(beforePos, afterPos, expectedBoardAfterMoveFunc(), playerId).SetName(
				$"{nameof(Move_piece_on_board)} Pawn from {beforePos} to {afterPos} with board after move being {expectedBoardAfterMoveFunc.Method.Name}");
		}
	}

	[TestCaseSource(nameof(MovePieceCases))]
	public void Move_piece_on_board(Position beforePos, Position afterPos, string expectedTilesAfterMove, int playerId)
	{
		var (tilesBefore, _, tilesByPlayerBefore) = Board.Create(StandardVariant.BoardAtStart());
		var twp = (TileWithPiece)Board.GetTile(beforePos, tilesBefore);

		var (beforeMoveTile, afterMoveTile, tilesAfterMove, tilesByPlayerAfterMove) = Board.MovePiece(twp, afterPos, tilesBefore, tilesByPlayerBefore[playerId]);
		Assert.That(beforeMoveTile, Is.EqualTo(new Tile(beforePos)));
		Assert.That(afterMoveTile, Is.EqualTo(twp with { Position = afterPos }));
		var (expTilesAfterMove, _, expTilesByPlayers)  = Board.ConvertBoardStringToTiles(expectedTilesAfterMove);
		Assert.That(tilesAfterMove, Is.EqualTo(expTilesAfterMove));
		var expTileByPlayer = expTilesByPlayers[playerId];
		Assert.That(tilesByPlayerAfterMove, Is.EqualTo(expTileByPlayer));
	}
	
}