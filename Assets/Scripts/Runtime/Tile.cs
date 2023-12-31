using System;

namespace Chess
{
	public class Tile
	{
		public Position Position { get; }
		public Piece Piece { get; }

		public Tile(Position position, Piece piece = null)
		{
			Position = position;
			Piece = piece;
		}
		
		public bool HasPiece()
		{
			return Piece != null;
		}

	}
}
