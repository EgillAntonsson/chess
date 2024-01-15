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
			const string standardBoardStart = "when Standard board in start position.";
			var pos = new Position(1, 0);
			const int playerIdToMove = 1;
			var moveAdditionLastPlayed = Position.None;
			yield return new TestCaseData(pos, StandardVariant.TilesStandard(), playerIdToMove, moveAdditionLastPlayed, new List<Position>
			{
				new(0, 2),
				new(2, 2)
			}).SetName($"{nameof(Find_valid_moves_on_board_for)} Knight on {pos} {standardBoardStart}");
			
			pos = new Position(0, 1);
			yield return new TestCaseData(pos, StandardVariant.TilesStandard(), playerIdToMove, moveAdditionLastPlayed, new List<Position>
			{
				new(0, 2),
				new(0, 3)
			}).SetName($"{nameof(Find_valid_moves_on_board_for)} Pawn on {pos} {standardBoardStart}");
			
			pos = new Position(4, 3);
			yield return new TestCaseData(pos, Tiles_Kings_Opening(), playerIdToMove, moveAdditionLastPlayed, new List<Position>
			{
				new(4, 4)
			}).SetName($"{nameof(Find_valid_moves_on_board_for)} Pawn on {pos} when {nameof(Tiles_Kings_Opening)}");
			
			pos = new Position(0, 1);
			yield return new TestCaseData(pos, Tiles_Kings_Opening(), playerIdToMove, moveAdditionLastPlayed, new List<Position>
			{
				new(0, 2),
				new(0, 3)
			}).SetName($"{nameof(Find_valid_moves_on_board_for)} Pawn on {pos} when {nameof(Tiles_Kings_Opening)}");
			
			moveAdditionLastPlayed = new Position(3, 4);
			pos = new Position(4, 4);
			yield return new TestCaseData(pos, Tiles_En_Passant(), playerIdToMove, moveAdditionLastPlayed, new List<Position>
			{
				new(4, 5),
				new(3, 5), // En Passant
			}).SetName($"{nameof(Find_valid_moves_on_board_for)} Pawn on {pos} when {nameof(Tiles_En_Passant)}");
		}
	}
	
	public static string Tiles_Kings_Opening()
	{
		return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 __ P2 P2 P2 P2
__ __ __ P2 __ __ __ __
__ __ __ __ __ __ __ __
__ __ __ __ P1 __ __ __
__ __ __ __ __ __ __ __
P1 P1 P1 P1 __ P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
	}
	
	public static string Tiles_En_Passant()
	{
		return @"
R2 N2 B2 Q2 K2 B2 N2 R2
__ P2 P2 __ P2 P2 P2 P2
__ __ __ __ __ __ __ __
P2 __ __ P2 P1 __ __ __
__ __ __ __ __ __ __ __
__ __ __ __ __ __ __ __
P1 P1 P1 P1 __ P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
	}
	
	[TestCaseSource(nameof(ValidMoveCases))]
	public void Find_valid_moves_on_board_for(Position piecePos, string tilesAtCurrent, int playerIdToMove, Position moveAdditionLastPlayed, IEnumerable<Position> expectedMoves)
	{
		var board = new Board(StandardVariant.TilesStandard());
		var twp = (TileWithPiece)Board.GetTile(piecePos, tilesAtCurrent);
		
		var validMoves = board.FindValidMoves(twp, StandardVariant.ValidMovesByTypeStandard, playerIdToMove, moveAdditionLastPlayed, tilesAtCurrent);
		
		AssertArraysAreEqual(validMoves, expectedMoves);
	}

	public static IEnumerable<TestCaseData> MovePieceCases
	{
		get
		{
			var beforePos = new Position(0, 1);
			var afterPos = new Position(0, 2);
			const int playerIdToMove = 1;
			var expectedIsFirstMoveAddition = false;
			yield return new TestCaseData(beforePos, afterPos, StandardVariant.TilesStandard(), playerIdToMove, expectedIsFirstMoveAddition).SetName($"{nameof(Move_piece_on_board)} Pawn from {beforePos} to {afterPos} when {nameof(StandardVariant.TilesStandard)}");
			
			afterPos = new Position(0, 3);
			expectedIsFirstMoveAddition = true;
			yield return new TestCaseData(beforePos, afterPos, StandardVariant.TilesStandard(), playerIdToMove, expectedIsFirstMoveAddition).SetName($"{nameof(Move_piece_on_board)} Pawn from {beforePos} to {afterPos} when {nameof(StandardVariant.TilesStandard)}");
		}
	}

	[TestCaseSource(nameof(MovePieceCases))]
	public void Move_piece_on_board(Position beforePos, Position afterPos, string tilesAtCurrent, int playerIdToMove, bool expectedIsFirstMoveAddition)
	{
		var board = new Board(StandardVariant.TilesStandard());
		var twp = (TileWithPiece)Board.GetTile(beforePos, tilesAtCurrent);

		var (beforeMoveTile, afterMoveTile, isFirstMoveAddition) = board.MovePiece(twp, afterPos, StandardVariant.ValidMovesByTypeStandard, playerIdToMove);
		Assert.That(beforeMoveTile, Is.EqualTo(new Tile(beforePos)));
		Assert.That(afterMoveTile, Is.EqualTo(twp with { Position = afterPos }));
		Assert.That(isFirstMoveAddition, Is.EqualTo(expectedIsFirstMoveAddition));
	}
	
	public static void AssertArraysAreEqual<T>(IEnumerable<T> actual, IEnumerable<T> expected)
	{
		var expectedArr = expected as T[] ?? expected.OrderBy(x => x).ToArray();
		var actualArr = actual as T[] ?? actual.OrderBy(x => x).ToArray();
		if (expectedArr.Length != actualArr.Length)
		{
			Assert.Fail($"Arrays have different lengths. Expected array length: {expectedArr.Length}, Actual array length: {actualArr.Length}.\nExpected array: {string.Join(", ", expectedArr)}.\nActual array: {string.Join(", ", actualArr)}");
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