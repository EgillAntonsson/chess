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
		var (tiles, tileByStartPos, posByTileByPlayer) = Board.Create(StandardVariant.BoardAtStart());

		Assert.That(tiles.Length, Is.EqualTo(64));
		Assert.That(tiles[0, 0], Is.EqualTo(new TileWithPiece(new Position(0, 0), new Piece(PieceType.Rook, 1))));

		Assert.That(tileByStartPos[new Position(0, 0)], Is.EqualTo(new TileWithPiece(new Position(0, 0), new Piece(PieceType.Rook, 1))));
		Assert.That(tileByStartPos[new Position(0, 1)], Is.EqualTo(new TileWithPiece(new Position(0, 1), new Piece(PieceType.Pawn, 1))));

		// Assert.That(posByTileByPlayer[1][new TileWithPiece(new Position(0, 0), new Piece(PieceType.Rook, 1))], Is.EqualTo(new Position(0, 0)));
		// Assert.That(posByTileByPlayer[2][new TileWithPiece(new Position(0, 7), new Piece(PieceType.Rook, 2))], Is.EqualTo(new Position(0, 7)));
	}

	public static IEnumerable<TestCaseData> ValidMoveCases
	{
		get
		{
			var pos = new Position(1, 0);
			var playerIdToMove = 1;
			Func<string> currentBoardFunc = StandardVariant.BoardAtStart;
			IEnumerable<Position> expectedMoves = new Position[] { new(0, 2), new(2, 2) };
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.Knight} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(0, 1);
			expectedMoves = new Position[] { new(0, 2), new(0, 3) };
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.Pawn} on {pos} when {currentBoardFunc.Method.Name}");

			currentBoardFunc = Notation_1_e4_c5;
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.Pawn} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(4, 3);
			expectedMoves = new Position[] { new(4, 4) };
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.Pawn} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(4, 3);
			currentBoardFunc = Notation_1_e4_e5;
			expectedMoves = Enumerable.Empty<Position>();
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.Pawn} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(4, 0);
			currentBoardFunc = StandardVariant.BoardAtStart;
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.King} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(5, 0);
			currentBoardFunc = StandardVariant.BoardAtStart;
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.Bishop} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(0, 0);
			currentBoardFunc = StandardVariant.BoardAtStart;
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.Rook} on {pos} when {currentBoardFunc.Method.Name}");

			pos = new Position(3, 0);
			currentBoardFunc = StandardVariant.BoardAtStart;
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, Enumerable.Empty<Position>()).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.Queen} on {pos} when {currentBoardFunc.Method.Name}");

			playerIdToMove = 2;
			pos = new Position(3, 7);
			currentBoardFunc = Quickest_Win__Player2_To_Move;
			expectedMoves = new Position[]
			{
				new(4, 6),
				new(5, 5),
				new(6, 4),
				new(7, 3)
			};
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.Queen} on {pos} when {currentBoardFunc.Method.Name}");

			playerIdToMove = 1;
			pos = new Position(4, 0);
			currentBoardFunc = Notation_1_e4_c5;
			expectedMoves = new Position[]
			{
				new(4, 1),
			};
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.King} on {pos} when {currentBoardFunc.Method.Name}");

			playerIdToMove = 1;
			pos = new Position(4, 0);
			currentBoardFunc = Player1_CheckMate;
			expectedMoves = new Position[] { };
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.King} on {pos} when {currentBoardFunc.Method.Name}");
			
			playerIdToMove = 1;
			pos = new Position(4, 0);
			currentBoardFunc = Check_but_king_can_move;
			expectedMoves = new Position[] { new (5, 1) };
			yield return new TestCaseData(pos, currentBoardFunc(), playerIdToMove, expectedMoves).SetName(
				$"{nameof(Find_valid_moves_on_board_for)} {PieceType.King} on {pos} when {currentBoardFunc.Method.Name}");
		}
	}

	[TestCaseSource(nameof(ValidMoveCases))]
	public void Find_valid_moves_on_board_for(Position piecePos, string tilesAtCurrent, int playerIdToMove, IEnumerable<Position> expectedMoves)
	{
		var (_, pieceTypeByStartPositions, _) = Board.Create(StandardVariant.BoardAtStart());
		var (tiles, _, tilesByPlayer) = Board.ConvertBoardStringToTiles(tilesAtCurrent);
		var twp = (TileWithPiece)Board.GetTile(piecePos, tiles);


		var validMoves = Board.FindMoves(twp,
			StandardVariant.ValidMovesByTypeStandard,
			playerIdToMove,
			tiles,
			pieceTypeByStartPositions,
			ChessBoard.GetOpponentTiles(tilesByPlayer, playerIdToMove));

		AssertArraysAreEqual(validMoves, expectedMoves);
	}

	[Test]
	public void Is_not_in_check_at_start()
	{
		var (tiles, tilesByStartPos, tilesByPlayer) = Board.Create(StandardVariant.BoardAtStart());
		var kingTile = new TileWithPiece(new Position(4, 0), new Piece(PieceType.King, 1));
		const int playerId = 1;
		
		var opponentCheck = Board.IsInCheck(kingTile, StandardVariant.ValidMovesByTypeStandard, ChessBoard.GetOpponentTiles(tilesByPlayer, playerId), tiles, tilesByStartPos);

		Assert.That(opponentCheck, Is.EqualTo(CheckType.NoCheck));
	}

	[Test]
	public void Is_check_mate()
	{
		Board.Create(StandardVariant.BoardAtStart());
		var (tiles, tilesByStartPos, tilesByPlayer) = Board.ConvertBoardStringToTiles(Player1_CheckMate());
		var kingTile = new TileWithPiece(new Position(4, 0), new Piece(PieceType.King, 1));
		const int playerId = 1;
		
		var opponentCheck = Board.IsInCheck(kingTile, StandardVariant.ValidMovesByTypeStandard, ChessBoard.GetOpponentTiles(tilesByPlayer, playerId), tiles, tilesByStartPos);

		Assert.That(opponentCheck, Is.EqualTo(CheckType.CheckMate));
	}

	public static IEnumerable<TestCaseData> MovePieceCases
	{
		get
		{
			var beforePos = new Position(0, 1);
			var afterPos = new Position(0, 2);
			yield return new TestCaseData(beforePos, afterPos, StandardVariant.BoardAtStart()).SetName(
				$"{nameof(Move_piece_on_board)} Pawn from {beforePos} to {afterPos} when {nameof(StandardVariant.BoardAtStart)}");

			afterPos = new Position(0, 3);
			yield return new TestCaseData(beforePos, afterPos, StandardVariant.BoardAtStart()).SetName(
				$"{nameof(Move_piece_on_board)} Pawn from {beforePos} to {afterPos} when {nameof(StandardVariant.BoardAtStart)}");
		}
	}

	[TestCaseSource(nameof(MovePieceCases))]
	public void Move_piece_on_board(Position beforePos, Position afterPos, string tilesAtCurrent)
	{
		var (_, pieceTypeByStartPositions, _) = Board.Create(StandardVariant.BoardAtStart());
		var (tiles, _, _) = Board.ConvertBoardStringToTiles(tilesAtCurrent);
		var twp = (TileWithPiece)Board.GetTile(beforePos, tiles);

		var (beforeMoveTile, afterMoveTile) = Board.MovePiece(twp, afterPos, tiles);
		Assert.That(beforeMoveTile, Is.EqualTo(new Tile(beforePos)));
		Assert.That(afterMoveTile, Is.EqualTo(twp with { Position = afterPos }));
	}

	public static void AssertArraysAreEqual<T>(IEnumerable<T> actual, IEnumerable<T> expected)
	{
		var expectedArr = expected as T[] ?? expected.OrderBy(x => x).ToArray();
		var actualArr = actual as T[] ?? actual.OrderBy(x => x).ToArray();
		if (expectedArr.Length != actualArr.Length)
		{
			Assert.Fail(
				$"Arrays have different lengths. Expected array length: {expectedArr.Length}, Actual array length: {actualArr.Length}.\nExpected array: {string.Join(", ", expectedArr)}.\nActual array: {string.Join(", ", actualArr)}");
		}

		for (int i = 0; i < expectedArr.Length; i++)
		{
			if (!expectedArr[i].Equals(actualArr[i]))
			{
				Assert.Fail($"Arrays differ at index {i}. Expected value: {expectedArr[i]}, Actual value: {actualArr[i]}");
			}
		}
	}

	// Notation names are 1 based.

	public static string Notation_1_e4_c5()
	{
		return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 -- P2 P2 P2 P2 P2
-- -- -- -- -- -- -- --
-- -- P2 -- -- -- -- --
-- -- -- -- P1 -- -- --
-- -- -- -- -- -- -- --
P1 P1 P1 P1 -- P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
	}

	public static string Notation_1_e4_e5()
	{
		return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 P2 -- P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- -- P2 -- -- --
-- -- -- -- P1 -- -- --
-- -- -- -- -- -- -- --
P1 P1 P1 P1 -- P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
	}

	public static string Check_but_king_can_move()
	{
		return @"
R2 N2 B2 -- K2 B2 N2 R2
P2 P2 P2 -- Q2 P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- P1 -- -- -- --
-- -- -- -- -- P1 -- --
-- -- -- -- -- -- -- --
P1 P1 P1 P1 -- -- P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
	}

	public static string Quickest_Win__Player2_To_Move()
	{
		return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 P2 -- P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- -- P2 -- -- --
-- -- -- -- -- -- P1 --
-- -- -- -- -- P1 -- --
P1 P1 P1 P1 P1 -- -- P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
	}

	public static string Player1_CheckMate()
	{
		return @"
R2 N2 B2 -- K2 B2 N2 R2
P2 P2 P2 P2 -- P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- -- P2 -- -- --
-- -- -- -- -- -- P1 Q2
-- -- -- -- -- P1 -- --
P1 P1 P1 P1 P1 -- -- P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
	}
}