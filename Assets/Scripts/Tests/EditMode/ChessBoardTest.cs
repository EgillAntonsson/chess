using System;
using System.Collections.Generic;
using System.Linq;
using Chess;
using NUnit.Framework;

public class ChessBoardTest
{
	public static IEnumerable<TestCaseData> FindMove()
	{
		Func<string> currentBoardFunc = BoardTileString.Check_but_king_can_move;
		var playerId = 1;
		var expectedPos = new Position[] { new(3, 0), new(3, 1), new(5, 0), new(5, 1) };
		var expectedMoves = new Dictionary<TileWithPiece, IEnumerable<Position>>();
		expectedMoves.Add(new TileWithPiece(new Position(4, 0), new Piece(PieceType.King, playerId)), expectedPos);
		var caseName = $"{nameof(Find_moves_for_all_pieces)} when {currentBoardFunc.Method.Name}".Replace('_', ' ');
		yield return new TestCaseData(currentBoardFunc(), playerId, expectedMoves).SetName(caseName);

		currentBoardFunc = BoardTileString.Check_but_piece_can_defend;
		playerId = 1;
		expectedPos = new Position[] { new(4, 1) };
		expectedMoves = new Dictionary<TileWithPiece, IEnumerable<Position>>();
		expectedMoves.Add(new TileWithPiece(new Position(3, 0), new Piece(PieceType.Queen, playerId)), expectedPos);
		expectedMoves.Add(new TileWithPiece(new Position(5, 0), new Piece(PieceType.Bishop, playerId)), expectedPos);
		expectedMoves.Add(new TileWithPiece(new Position(6, 0), new Piece(PieceType.Knight, playerId)), expectedPos);
		caseName = $"{nameof(Find_moves_for_all_pieces)} when {currentBoardFunc.Method.Name}".Replace('_', ' ');
		yield return new TestCaseData(currentBoardFunc(), playerId, expectedMoves).SetName(caseName);
	}

	[TestCaseSource(nameof(FindMove))]
	public void Find_moves_for_all_pieces(string currentBoard, int playerId, Dictionary<TileWithPiece, IEnumerable<Position>> expMoves)
	{
		var chessboard = new ChessBoard();
		chessboard.Create(StandardVariant.BoardAtStart());
		var (tiles, _, tilesByPlayer) = chessboard.InjectBoard(currentBoard);

		foreach (var twp in tilesByPlayer[playerId])
		{
			var foundMoves = chessboard.FindMoves(twp,
					true,
					StandardVariant.CheckablePieceTypeStandard,
					StandardVariant.ValidMovesByTypeStandard,
					playerId);

			// will default to empty positions
			var expectedMoves = Enumerable.Empty<Position>();
			if (expMoves.TryGetValue(twp, out var move)) 
			{
				expectedMoves = move;
			}

			var (areEqual, failMessage) = TestUtil.AreArraysEqual(foundMoves, expectedMoves);
			Assert.IsTrue(areEqual, $"For {twp}: {failMessage}");
		}
	}
}