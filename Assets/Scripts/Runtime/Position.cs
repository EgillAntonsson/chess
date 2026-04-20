using System;

namespace Chess
{
	public readonly record struct Position(int Column, int Row) : IComparable<Position>
	{
		public enum Axis
		{
			Column = 0,
			Row = 1
		}

		public int CompareTo(Position other)
		{
			var columnComparison = Column.CompareTo(other.Column);
			return columnComparison != 0 ? columnComparison : Row.CompareTo(other.Row);
		}

		public static Position operator +(Position left, Position right) =>
			new(left.Column + right.Column, left.Row + right.Row);

		public static Position operator -(Position left, Position right) =>
			new(left.Column - right.Column, left.Row - right.Row);

		public static Position operator *(Position pos, int multiplier) =>
			new(pos.Column * multiplier, pos.Row * multiplier);

		public static int GridDistance(Position pos1, Position pos2) =>
			Math.Max(Math.Abs(pos1.Column - pos2.Column), Math.Abs(pos1.Row - pos2.Row));

		public static Position GridNormal(Position pos1, Position pos2)
		{
			var col = pos2.Column - pos1.Column;
			var row = pos2.Row - pos1.Row;
			var max = Math.Max(Math.Abs(col), Math.Abs(row));
			return max == 0 ? new Position(0, 0) : new Position(col / max, row / max);
		}
	}
}
