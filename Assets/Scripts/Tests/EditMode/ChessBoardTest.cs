using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chess;
using NUnit.Framework;

public class ChessBoardTest
{
	[Test]
	public void Find_moves_when_king_in_check_but_can_find_moves()
	{
		const int playerIdToMove = 1;
		var pos = new Position(4, 0);
		var expectedMoves = new Position[] { new(3, 0), new(3, 1), new(5, 0), new(5, 1) };

		var chessboard = new ChessBoard();
		chessboard.Create(StandardVariant.BoardAtStart());
		var (tiles, _, tilesByPlayer) = chessboard.InjectBoard(BoardTileString.Check_but_king_can_move());
		var kingTile = (TileWithPiece)Board.GetTile(pos, tiles);
		const bool isCheckablePiece = true;
		
		var validMoves = chessboard.FindMoves(kingTile, true,
				isCheckablePiece,
				StandardVariant.ValidMovesByTypeStandard,
				playerIdToMove);
		
		TestUtil.AssertArraysAreEqual(validMoves, expectedMoves);

		// No moves should be found for the rest of player pieces.
		Position[] noPositionsFound = { };
		foreach (var twp in tilesByPlayer[playerIdToMove].Where(t => t != kingTile))
		{
			
			var vm = chessboard.FindMoves(twp, true,
					false,
					StandardVariant.ValidMovesByTypeStandard,
					playerIdToMove);
			var (areEqual, failMessage) = TestUtil.AreArraysEqual(vm, noPositionsFound);
			Assert.IsTrue(areEqual, $"For {twp}: {failMessage}");
		}
	}
}