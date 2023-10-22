using System;

public readonly struct Position : IEquatable<Position> {
	
	public int Row { get; }
	public int Column { get; }

	public Position(int row, int column)
	{
		Row = row;
		Column = column;
		Validate();
	}

	private void Validate()
	{
		ValidateCoord(Row);
		ValidateCoord(Column);
	}
	
	private static void ValidateCoord(int coord)
	{
		const int lowestValidValue = 0;
		if (coord < lowestValidValue)
		{
			throw new ArgumentOutOfRangeException(nameof(coord),
				$"Value {coord} is invalid, it should be equal or higher than {lowestValidValue}");
		}
	}

	public bool Equals(Position other)
	{
		return Row == other.Row && Column == other.Column;
	}

	public override bool Equals(object obj)
	{
		return obj is Position other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Row, Column);
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
