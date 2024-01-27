using System;
using System.Collections.Generic;
using System.Linq;
using Chess;
using NUnit.Framework;

public class BoardTest
{
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
		}
	}

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

	[TestCaseSource(nameof(ValidMoveCases))]
	public void Find_valid_moves_on_board_for(Position piecePos, string tilesAtCurrent, int playerIdToMove, IEnumerable<Position> expectedMoves)
	{
		var board = new Board();
		var (_, pieceTypeByStartPositions) = Board.Create(StandardVariant.BoardAtStart());
		var boardTiles = Board.ConvertToTile2dArray(tilesAtCurrent);
		var twp = (TileWithPiece)Board.GetTile(piecePos, boardTiles);


		var validMoves = Board.FindValidMoves(twp, StandardVariant.ValidMovesByTypeStandard, playerIdToMove, boardTiles, pieceTypeByStartPositions);

		AssertArraysAreEqual(validMoves, expectedMoves);
	}
	
	[Test]
	public void IsCheckMate()
	{
		Board.Create(StandardVariant.BoardAtStart());
		var boardTiles = Board.ConvertToTile2dArray(Player1_CheckMate());

		var opponentCheck = Board.IsInCheck(1, boardTiles);

		Assert.That(opponentCheck.checktype, Is.EqualTo(CheckType.CheckMate));
		Assert.That(opponentCheck.checkTile, Is.EqualTo(new Tile(new Position(0, 4))));
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
		
		var board = new Board();
		var (_, pieceTypeByStartPositions) = Board.Create(StandardVariant.BoardAtStart());
		var boardTiles = Board.ConvertToTile2dArray(tilesAtCurrent);
		var twp = (TileWithPiece)Board.GetTile(beforePos, boardTiles);

		var (beforeMoveTile, afterMoveTile) = Board.MovePiece(twp, afterPos, boardTiles);
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
}