using System;

namespace Chess
{
	public record Move(Position Position, MoveType MoveType, MoveCaptureFlag MoveCaptureFlag, MoveConstraint MoveConstraint = MoveConstraint.None);

	[Flags]
	public enum MoveCaptureFlag
	{
		Move = 1 << 0,
		Capture = 2 << 1
	}

	public enum MoveType
	{
		Basic = 0,
		Infinite = 1
	}

	public enum MoveConstraint
	{
		None = 0,
		FirstMoveOnly = 1,
		CanMoveIfNotThreatenedCapture = 2,
		CastlingMove = 3
	}
}