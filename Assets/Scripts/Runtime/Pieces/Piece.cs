using System;

namespace Chess
{
	public readonly struct Piece : IEquatable<Piece>
	{
		public PieceType Type { get; }
		public int PlayerId { get; }

		public Piece(PieceType type, int playerId)
		{
			Type = type;
			PlayerId = playerId;
		}
		
		public override string ToString()
		{
			return $"Piece: {Type}, {PlayerId}";
		}
		public bool Equals(Piece other)
		{
			return Type == other.Type && PlayerId == other.PlayerId;
		}

		public override bool Equals(object obj)
		{
			return obj is Piece other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine((int)Type, PlayerId);
		}

		public static bool operator ==(Piece left, Piece right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Piece left, Piece right)
		{
			return !left.Equals(right);
		}
	}
}
