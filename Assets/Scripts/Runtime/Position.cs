using System;

namespace Chess
{
	public readonly struct Position : IEquatable<Position>, IComparable<Position>
	{
		public static Position None => new Position(int.MinValue, int.MinValue);
		public int Column { get; }
		public int Row { get; }

		public Position(int column, int row)
		{
			Column = column;
			Row = row;
		}

		public override string ToString()
		{
			return $"Position: {Column}, {Row}";
		}

		public int CompareTo(Position other)
		{
			var columnComparison = Column.CompareTo(other.Column);
			return columnComparison != 0 ? columnComparison : Row.CompareTo(other.Row);
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
			return !(left.Equals(right));
		}

		public static Position operator +(Position left, Position right)
		{
			return new Position(left.Column + right.Column, left.Row + right.Row);
		}
		
		public static Position operator -(Position left, Position right)
		{
			return new Position(left.Column - right.Column, left.Row - right.Row);
		}
		
		public static Position operator *(Position pos, int multiplier)
		{
			return new Position(pos.Column * multiplier, pos.Row * multiplier);
		}
		
		public static int GridDistance(Position pos1, Position pos2)
		{
			return Math.Max(Math.Abs(pos1.Column - pos2.Column), Math.Abs(pos1.Row - pos2.Row));
		}
		
		public static Position GridNormal(Position pos1, Position pos2)
		{
			var col = pos2.Column - pos1.Column;
			var row = pos2.Row - pos1.Row;
			var max = Math.Max(Math.Abs(col), Math.Abs(row));
			return max == 0 ? new Position(0, 0) : new Position(col / max, row / max);
		}
	}
}