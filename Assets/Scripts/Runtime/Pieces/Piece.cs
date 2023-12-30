using System;

namespace Chess
{
	public class Piece : IEquatable<Piece>
	{
		public PieceType Type { get; }
		public Color Color { get; }

		public Piece(PieceType type, Color color)
		{
			Type = type;
			Color = color;
		}

		public bool Equals(Piece other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Type == other.Type && Color == other.Color;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Piece)obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine((int)Type, (int)Color);
		}

		public static bool operator ==(Piece left, Piece right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Piece left, Piece right)
		{
			return !Equals(left, right);
		}
	}
}
