using System;

namespace Chess
{
	public record Move(Position Position, MoveType MoveType, MoveCaptureFlag MoveCaptureFlag, MoveConstraint MoveConstraint = MoveConstraint.None);

	// public record MoveWithPieceSwitch(PieceType PieceTypeSwitch, Position Position, MoveType MoveType, MoveCaptureFlag MoveCaptureFlag, MoveConstraint MoveConstraint = MoveConstraint.None)
		// : Move(Position, MoveType, MoveCaptureFlag, MoveConstraint);

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

	public enum MoveConstraint
	{
		None = 0,
		FirstMoveOnly = 1,
		CanMoveIfNotThreatenedCapture = 2
	}
}