
using System.Collections.Generic;

namespace Chess
{
	public static class ValidMoves
	{
		public static IEnumerable<Position> Knight()
		{
			return new List<Position>
			{
				new Position(1, 2),
				new Position(2, 1),
				new Position(2, -1),
				new Position(1, -2),
				new Position(-1, -2),
				new Position(-2, -1),
				new Position(-2, 1),
				new Position(-1, 2)
			};
		}
		
		public static  IEnumerable<Position> Pawn(int playerId)
		{
			return new List<Position>
			{
				new Position(0, playerId),
				new Position(0, playerId * 2),
				new Position(1, playerId),
				new Position(-1, playerId)
			};
		}
	}
}