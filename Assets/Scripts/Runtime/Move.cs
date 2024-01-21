using System;

namespace Chess
{
	public record Move(Position Position, MoveType MoveType, MoveCaptureFlag MoveCaptureFlag, MoveConstraints MoveConstraints = MoveConstraints.None);

	[Flags]
	public enum MoveCaptureFlag
	{
		Move = 1 << 0,
		Capture = 2 << 1
	}

	public enum MoveType
	{
		Basic = 0,
		Infinite = 1,
		// Jump = 2
	}

	public enum MoveConstraints
	{
		None = 0,
		FirstMoveOnly = 1
	}
}