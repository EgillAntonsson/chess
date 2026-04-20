namespace Chess
{
	public record Move(Position Position, MoveType MoveType, MoveCaptureFlag MoveCaptureFlag, MoveConstraint MoveConstraint = MoveConstraint.None);
}
