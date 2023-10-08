public class King
{
	public Position Position { get; }
	
	public King(Position position)
	{
		Position = position;
	}
	
	// public Position[] GetMoveDefinitione()
	// {
	// 	return new[]
	// 	{
	// 		new Position(1, 0),
	// 		new Position(-1, 0),
	// 		new Position(0, 1),
	// 		new Position(0, -1),
	// 		new Position(1, 1),
	// 		new Position(-1, 1),
	// 		new Position(1, -1),
	// 		new Position(-1, -1)
	// 	};
	// }
}
