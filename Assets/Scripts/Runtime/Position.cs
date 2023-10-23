using System;

public readonly struct Position
{
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
}