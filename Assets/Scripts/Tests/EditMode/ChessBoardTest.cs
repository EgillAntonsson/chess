using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chess;
using NUnit.Framework;

public class ChessBoardTest
{
	[Test]
	public void In_check_but_king_can_find_moves()
	{
		const int playerIdToMove = 1;
		var pos = new Position(4, 0);
		var expectedMoves = new Position[] { new(3, 0), new(3, 1), new(5, 0), new(5, 1) };

		var chessboard = new ChessBoard();
		chessboard.Create(StandardVariant.BoardAtStart());
		var (tiles, _, tilesByPlayer) = chessboard.InjectBoard(BoardTileString.Check_but_king_can_move());
		var kingTile = (TileWithPiece)Board.GetTile(pos, tiles);

		var validMoves = chessboard.FindMoves(kingTile,
				true,
				StandardVariant.CheckablePieceTypeStandard,
				StandardVariant.ValidMovesByTypeStandard,
				playerIdToMove);

		TestUtil.AssertArraysAreEqual(validMoves, expectedMoves);

		// No moves should be found for the rest of player pieces.
		Position[] noPositionsFound = { };
		foreach (var twp in tilesByPlayer[playerIdToMove].Where(t => t != kingTile))
		{
			var vm = chessboard.FindMoves(twp,
					true,
					StandardVariant.CheckablePieceTypeStandard,
					StandardVariant.ValidMovesByTypeStandard,
					playerIdToMove);
			var (areEqual, failMessage) = TestUtil.AreArraysEqual(vm, noPositionsFound);
			Assert.IsTrue(areEqual, $"For {twp}: {failMessage}");
		}
	}

	[Test]
	public void In_check_but_pieces_can_defend()
	{
		var chessboard = new ChessBoard();
		chessboard.Create(StandardVariant.BoardAtStart());
		var (tiles, _, tilesByPlayer) = chessboard.InjectBoard(BoardTileString.Check_but_piece_can_defend());

		const int playerIdToMove = 1;
		var pos = new Position(4, 0);
		var expectedMoves = new Dictionary<TileWithPiece, IEnumerable<Position>>();
		var expectedPos = new List<Position> { new Position(4, 1) };
		expectedMoves.Add(new TileWithPiece(new Position(3, 0), new Piece(PieceType.Queen, 1)), expectedPos);
		expectedMoves.Add(new TileWithPiece(new Position(5, 0), new Piece(PieceType.Bishop, 1)), expectedPos);
		expectedMoves.Add(new TileWithPiece(new Position(6, 0), new Piece(PieceType.Knight, 1)), expectedPos);
		
		foreach (var twp in tilesByPlayer[playerIdToMove])
		{
			var isCheckableTile = twp.Piece.Type == StandardVariant.CheckablePieceTypeStandard;	
			var vm = chessboard.FindMoves(twp,
					true,
					StandardVariant.CheckablePieceTypeStandard,
					StandardVariant.ValidMovesByTypeStandard,
					playerIdToMove);
			// will default to empty positions
			var expected = Enumerable.Empty<Position>();
			if (expectedMoves.ContainsKey(twp))
			{
				expected = expectedMoves[twp];
			}
				
			var (areEqual, failMessage) = TestUtil.AreArraysEqual( vm, expected);
			Assert.IsTrue(areEqual, $"For {twp}: {failMessage}");
		}
	}
	
	
}