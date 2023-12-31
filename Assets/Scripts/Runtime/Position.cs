using System;

namespace Chess
{
	public readonly struct Position : IEquatable<Position>
	{
		public int Column { get; }
		public int Row { get; }

		public Position(int column, int row)
		{
			Column = column;
			Row = row;
		}

		public bool Equals(Position other)
		{
			return Column == other.Column && Row == other.Row;
		}

		public override bool Equals(object obj)
		{
			return obj is Position other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Column, Row);
		}

		public static bool operator ==(Position left, Position right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Position left, Position right)
		{
			return !left.Equals(right);
		}
	}
}