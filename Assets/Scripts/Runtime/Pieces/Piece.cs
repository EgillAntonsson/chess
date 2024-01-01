using System;
using System.Collections.Generic;

namespace Chess
{
	public readonly struct Piece : IEquatable<Piece>
	{
		public PieceType Type { get; }
		public int PlayerId { get; }
		public Func<IEnumerable<Position>> ValidMoves { get; }

		public Piece(PieceType type, int playerId, Func<IEnumerable<Position>> validMoves = null)
		{
			Type = type;
			PlayerId = playerId;
			ValidMoves = validMoves;
		}
		
		public override string ToString()
		{
			return $"Piece: {Type}, {PlayerId}";
		}

		public bool Equals(Piece other)
		{
			return Type == other.Type && PlayerId == other.PlayerId && Equals(ValidMoves, other.ValidMoves);
		}

		public override bool Equals(object obj)
		{
			return obj is Piece other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine((int)Type, PlayerId, ValidMoves);
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
