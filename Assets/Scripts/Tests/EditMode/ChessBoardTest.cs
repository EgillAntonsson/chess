using System;
using System.Collections.Generic;
using System.Linq;
using Chess;
using NUnit.Framework;

public class ChessBoardTest
{
	[Test]
	public void create_and_validate_board()
	{
		var rules = new Rules();
		var chessboard = new ChessBoard(rules);
		var (tiles, tileByStartPos, tilesByPlayer) = chessboard.Create(rules.BoardAtStart);
		var (success, errorMsg) = ChessBoard.ValidateBoard(tiles, tileByStartPos, tilesByPlayer);
		Assert.That(success, Is.True);
		Assert.That(errorMsg, Is.Empty);
	}

	public static IEnumerable<TestCaseData> Find_moves_for_all()
	{
		Func<string> currentBoardFunc = BoardTileString.Check_but_king_can_move_but_not_castle;
		var playerId = 1;
		var expectedPos = new Position[] { new(3, 0), new(3, 1), new(5, 0), new(5, 1) };
		var expectedMoves = new Dictionary<TileWithPiece, IEnumerable<Position>>
		{
			{ new TileWithCheckablePiece(new Position(4, 0), new Piece(PieceType.King, playerId)), expectedPos }
		};
		var caseName = $"{nameof(Find_moves_for_all_pieces_when_in_check)} when {currentBoardFunc.Method.Name}".Replace('_', ' ');
		yield return new TestCaseData(currentBoardFunc(), playerId, expectedMoves).SetName(caseName);


		currentBoardFunc = BoardTileString.Check_but_piece_can_defend;
		playerId = 1;
		expectedPos = new Position[] { new(4, 1) };
		expectedMoves = new Dictionary<TileWithPiece, IEnumerable<Position>>
		{
			{ new TileWithPiece(new Position(3, 0), new Piece(PieceType.Queen, playerId)), expectedPos },
			{ new TileWithPiece(new Position(5, 0), new Piece(PieceType.Bishop, playerId)), expectedPos },
			{ new TileWithPiece(new Position(6, 0), new Piece(PieceType.Knight, playerId)), expectedPos }
		};
		caseName = $"{nameof(Find_moves_for_all_pieces_when_in_check)} when {currentBoardFunc.Method.Name}".Replace('_', ' ');
		yield return new TestCaseData(currentBoardFunc(), playerId, expectedMoves).SetName(caseName);
	}

	[TestCaseSource(nameof(Find_moves_for_all))]
	public void Find_moves_for_all_pieces_when_in_check(string currentBoard, int playerId, Dictionary<TileWithPiece, IEnumerable<Position>> expMoves)
	{
		var rules = new Rules();
		var chessboard = new ChessBoard(rules);
		var (_, _, _) = chessboard.Create(rules.BoardAtStart);
		var (_, tilesByPlayer) = chessboard.Create_ButNotUpdateStartPos(currentBoard);

		foreach (var twp in tilesByPlayer[playerId])
		{
			var foundMoves = chessboard.FindMoves(twp, true, playerId);

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

	public static IEnumerable<TestCaseData> Find_moves()
	{
		Func<string> currentBoardFunc = BoardTileString.Can_castle_king_side;
		var tileWithPiece = new TileWithCheckablePiece(new Position(4, 0), new Piece(PieceType.King, 1));
		var expectedMoves = new Position[]
		{
			new(4, 1),
			new(5, 0),
			new(6, 0)
		};
		var caseName = $"{nameof(Find_moves_when_not_in_check)} when {currentBoardFunc.Method.Name}".Replace('_', ' ');
		yield return new TestCaseData(currentBoardFunc(), tileWithPiece, expectedMoves).SetName(caseName);


		currentBoardFunc = BoardTileString.Can_castle_queen_side;
		tileWithPiece = new TileWithCheckablePiece(new Position(4, 0), new Piece(PieceType.King, 1));
		expectedMoves = new Position[]
		{
			new(4, 1),
			new(3, 1),
			new(3, 0),
			new(2, 0)
		};
		caseName = $"{nameof(Find_moves_when_not_in_check)} when {currentBoardFunc.Method.Name}".Replace('_', ' ');
		yield return new TestCaseData(currentBoardFunc(), tileWithPiece, expectedMoves).SetName(caseName);


		currentBoardFunc = BoardTileString.Can_castle_on_both_sides;
		tileWithPiece = new TileWithCheckablePiece(new Position(4, 0), new Piece(PieceType.King, 1));
		expectedMoves = new Position[]
		{
			new(4, 1),
			new(3, 1),
			new(3, 0),
			new(2, 0),
			new(5, 0),
			new(6, 0)
		};
		caseName = $"{nameof(Find_moves_when_not_in_check)} when {currentBoardFunc.Method.Name}".Replace('_', ' ');
		yield return new TestCaseData(currentBoardFunc(), tileWithPiece, expectedMoves).SetName(caseName);


		currentBoardFunc = BoardTileString.Can_not_castle_as_1st_intra_move_would_be_checked;
		tileWithPiece = new TileWithCheckablePiece(new Position(4, 0), new Piece(PieceType.King, 1));
		expectedMoves = new Position[]
		{
			new(4, 1),
		};
		caseName = $"{nameof(Find_moves_when_not_in_check)} when {currentBoardFunc.Method.Name}".Replace('_', ' ');
		yield return new TestCaseData(currentBoardFunc(), tileWithPiece, expectedMoves).SetName(caseName);


		currentBoardFunc = BoardTileString.Can_not_castle_as_2nd_intra_move_would_be_checked;
		tileWithPiece = new TileWithCheckablePiece(new Position(4, 0), new Piece(PieceType.King, 1));
		expectedMoves = new Position[]
		{
			new(4, 1),
			new(3, 1),
			new(3, 0)
		};
		caseName = $"{nameof(Find_moves_when_not_in_check)} when {currentBoardFunc.Method.Name}".Replace('_', ' ');
		yield return new TestCaseData(currentBoardFunc(), tileWithPiece, expectedMoves).SetName(caseName);
	}

	[TestCaseSource(nameof(Find_moves))]
	public void Find_moves_when_not_in_check(string currentBoard, TileWithPiece tileWithPiece, IEnumerable<Position> expectedMoves)
	{
		var rules = new Rules();
		var chessboard = new ChessBoard(rules);
		chessboard.Create(rules.BoardAtStart);
		chessboard.Create_ButNotUpdateStartPos(currentBoard);

		const bool isInCheck = false;
		var movePositions = chessboard.FindMoves(tileWithPiece, isInCheck, tileWithPiece.Piece.PlayerId);

		TestUtil.AssertArraysAreEqual(movePositions, expectedMoves);
	}

	[Test]
	public void Can_not_castle_after_king_has_moved()
	{
		var rules = new Rules();
		var chessboard = new ChessBoard(rules);
		chessboard.Create(rules.BoardAtStart);
		var (tiles, _) = chessboard.Create_ButNotUpdateStartPos(BoardTileString.Check_but_king_can_move_but_not_castle());
		// We assume we are moving to a found valid position from FindMoves method.
		// Move the king
		(_, tiles) = chessboard.MovePiece((TileWithPiece)tiles[4, 0], new Position(5, 0));
		
		// The opponent moves bishop in front of his\her queen .
		(_, tiles) = chessboard.MovePiece((TileWithPiece)tiles[2, 7], new Position(4, 5));

		// king moves back to his start position, as the opponent's bishop is in between the queen now.
		(_, tiles) = chessboard.MovePiece((TileWithPiece)tiles[5, 0], new Position(4, 0));
		
		// The opponent does a move that has no impact on what is being tested.
		(_, tiles) = chessboard.MovePiece((TileWithPiece)tiles[0, 6], new Position(0, 5));

		// Find moves for the king
		var movePositions = chessboard.FindMoves(tileWithPiece: (TileWithPiece)tiles[4, 0], isInCheck: false, playerId: 1);

		var expectedMoves = new Position[]
		{
			new(3, 0),
			new(3, 1),
			new(4, 1),
			new(5, 1),
			new(5, 0)
		};

		TestUtil.AssertArraysAreEqual(movePositions, expectedMoves);
	}
	
	[Test]
	public void Can_not_castle_after_rook_has_moved()
	{
		var rules = new Rules();
		var chessboard = new ChessBoard(rules);
		chessboard.Create(rules.BoardAtStart);
		var (tiles, _) = chessboard.Create_ButNotUpdateStartPos(BoardTileString.Can_castle_on_both_sides());
		// We assume we are moving to a found valid position from FindMoves method.
		// Move the left rook
		(_, tiles) = chessboard.MovePiece((TileWithPiece)tiles[0, 0], new Position(1, 0));
		
		// The opponent does a move that has no impact on what is being tested.
		(_, tiles) = chessboard.MovePiece((TileWithPiece)tiles[0, 5], new Position(0, 4));

		// Move the left rook back to start pos
		(_, tiles) = chessboard.MovePiece((TileWithPiece)tiles[1, 0], new Position(0, 0));
		
		// The opponent does a move that has no impact on what is being tested.
		(_, tiles) = chessboard.MovePiece((TileWithPiece)tiles[0, 4], new Position(0, 3));

		// Find moves for the king
		var movePositions = chessboard.FindMoves(tileWithPiece: (TileWithPiece)tiles[4, 0], isInCheck: false, playerId: 1);

		var expectedMoves = new Position[]
		{
			new(3, 0),
			new(3, 1),
			new(4, 1),
			new(5, 0),
			new(6, 0)
		};

		TestUtil.AssertArraysAreEqual(movePositions, expectedMoves);
	}
	
	[Test]
	public void Move_to_castling()
	{
		var rules = new Rules();
		var chessboard = new ChessBoard(rules);
		chessboard.Create(rules.BoardAtStart);
		var (tiles, _) = chessboard.Create_ButNotUpdateStartPos(BoardTileString.Can_castle_king_side());
	
		var (changedTiles, _) = chessboard.MovePiece((TileWithPiece)tiles[4, 0], new Position(6, 0));
	
		const int playerId = 1;
		var expectedCheckablePiece = new TileWithCheckablePiece(new Position(6, 0), new Piece(PieceType.King, playerId), true);
		var expectedCastlingPiece = new TileWithCastlingPiece(new Position(5, 0), new Piece(PieceType.Rook, playerId), true);

		Assert.That(changedTiles.First(t => t.Position == new Position(6, 0)), Is.EqualTo(expectedCheckablePiece));
		Assert.That(changedTiles.First(t => t.Position == new Position(5, 0)), Is.EqualTo(expectedCastlingPiece));
	}
	
}