using System;

namespace Chess
{
	public readonly struct Tile : IEquatable<Tile>
	{
		public Position Position { get; }
		public Piece Piece { get; }
		public TileType TileType { get; }
		public bool HasPiece => Piece.Type != PieceType.None;
		
		public Tile(Position position, Piece piece)
		{
			Position = position;
			Piece = piece;
			TileType = TileType.Normal;
		}

		public Tile(Position position)
		{
			Position = position;
			Piece = new Piece(PieceType.None, -1);
			TileType = TileType.Normal;
		}
		
		public Tile(Position position, Piece piece, TileType type)
		{
			Position = position;
			Piece = piece;
			TileType = type;
		}
		
		public bool IsValid()
		{
			return TileType != TileType.None;
		}

		public override string ToString()
		{
			return $"Tile: {Position}, {Piece}, {TileType}";
		}

		public bool Equals(Tile other)
		{
			return Position.Equals(other.Position) && Piece.Equals(other.Piece) && TileType == other.TileType;
		}

		public override bool Equals(object obj)
		{
			return obj is Tile other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Position, Piece, (int)TileType);
		}

		public static bool operator ==(Tile left, Tile right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Tile left, Tile right)
		{
			return !left.Equals(right);
		}
	}
}