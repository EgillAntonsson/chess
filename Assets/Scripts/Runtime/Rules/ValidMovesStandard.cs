using System;
using System.Collections.Generic;

namespace Chess
{
	public static class ValidMovesStandard
	{
		public static IEnumerable<Move> Knight()
		{
			var moves = new Move[]
			{
				new(new Position(1, 2), MoveType.Move | MoveType.Capture),
				new(new Position(2, 1), MoveType.Move | MoveType.Capture),
				new(new Position(2, -1), MoveType.Move | MoveType.Capture),
				new(new Position(1, -2), MoveType.Move | MoveType.Capture),
				new(new Position(-1, -2), MoveType.Move | MoveType.Capture),
				new(new Position(-2, -1), MoveType.Move | MoveType.Capture),
				new(new Position(-2, 1), MoveType.Move | MoveType.Capture),
				new(new Position(-1, 2), MoveType.Move | MoveType.Capture)
			};
			return moves;
		}

		public static IEnumerable<Move> Pawn(int playerId)
		{
			var row = playerId == 1 ? 1 : -1;
			return new Move[]
			{
				new(new Position(0, row), MoveType.Move),
				new(new Position(0, row * 2), MoveType.Move, MoveConstraint.FirstMoveOnly),
				new(new Position(1, row), MoveType.Capture),
				new(new Position(-1, row), MoveType.Capture)
			};
		}
	}

	public record Move(Position Position, MoveType MoveType, MoveConstraint MoveConstraint = MoveConstraint.None);

	[Flags]
	public enum MoveType
	{
		Move = 1 << 0,
		Capture = 2 << 1
	}

	public enum MoveConstraint
	{
		None = 0,
		FirstMoveOnly = 1,
	}
}