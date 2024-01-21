using System;
using System.Collections.Generic;

namespace Chess
{
	public static class ValidMovesStandard
	{
		public static IEnumerable<Move> Knight()
		{
			const MoveType moveType = MoveType.Basic;
			const MoveCaptureFlag moveAndCaptureFlagSet = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			return new Move[]
			{
				new(new Position(1, 2), moveType, moveAndCaptureFlagSet),
				new(new Position(2, 1), moveType, moveAndCaptureFlagSet),
				new(new Position(2, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(1, -2), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, -2), moveType, moveAndCaptureFlagSet),
				new(new Position(-2, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(-2, 1), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, 2), moveType, moveAndCaptureFlagSet)
			};
		}

		public static IEnumerable<Move> Pawn(int playerId)
		{
			var row = playerId == 1 ? 1 : -1;
			const MoveType moveType = MoveType.Basic;
			return new Move[]
			{
				new(new Position(0, row), moveType, MoveCaptureFlag.Move),
				new(new Position(0, row * 2), moveType, MoveCaptureFlag.Move, MoveConstraints.FirstMoveOnly),
				new(new Position(1, row), moveType, MoveCaptureFlag.Capture),
				new(new Position(-1, row), moveType, MoveCaptureFlag.Capture)
			};
		}

		public static IEnumerable<Move> Queen()
		{
			const MoveType moveType = MoveType.Infinite;
			const MoveCaptureFlag moveAndCaptureFlagSet = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			return new Move[]
			{
				new(new Position(0, 1), moveType, moveAndCaptureFlagSet),
				new(new Position(1, 1), moveType, moveAndCaptureFlagSet),
				new(new Position(1, 0), moveType, moveAndCaptureFlagSet),
				new(new Position(1, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(0, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, 0), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, 1), moveType, moveAndCaptureFlagSet)
			};
		}
	}
}