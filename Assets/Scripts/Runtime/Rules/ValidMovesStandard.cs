using System.Collections.Generic;

namespace Chess
{
	public static class ValidMovesStandard
	{
		public static IEnumerable<Move> Knight()
		{
			var moves = new Move[]
			{
				new(new Position(1, 2), MoveType.CaptureOrMove),
				new(new Position(2, 1), MoveType.CaptureOrMove),
				new(new Position(2, -1), MoveType.CaptureOrMove),
				new(new Position(1, -2), MoveType.CaptureOrMove),
				new(new Position(-1, -2), MoveType.CaptureOrMove),
				new(new Position(-2, -1), MoveType.CaptureOrMove),
				new(new Position(-2, 1), MoveType.CaptureOrMove),
				new(new Position(-1, 2), MoveType.CaptureOrMove)
			};
			return moves;
		}

		public static IEnumerable<Move> Pawn(int playerId)
		{
			var row = playerId == 1 ? 1 : -1;
			return new Move[]
			{
				new(new Position(0, row * 2), MoveType.FirstMoveAddition),
				new(new Position(0, row), MoveType.MoveOnly),
				new(new Position(1, row), MoveType.CaptureOnly),
				new(new Position(-1, row), MoveType.CaptureOnly)
			};
		}
	}

	public record Move(Position Position, MoveType MoveType);

	public enum MoveType
	{
		CaptureOrMove = 0,
		CaptureOnly = 1,
		MoveOnly = 2,
		FirstMoveAddition = 3
	}
}