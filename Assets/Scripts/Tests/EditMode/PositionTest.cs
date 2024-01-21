using Chess;
using NUnit.Framework;

public class PositionTest
{
	[Test]
	public void Column_and_row_are_initialized()
	{
		var position = new Position(0, 1);
		Assert.That(position.Column, Is.EqualTo(0));
		Assert.That(position.Row, Is.EqualTo(1));
	}

	[Test]
	public void Are_equal_when_with_the_same_row_and_column()
	{
		var pos1 = new Position(0, 0);
		var pos2 = new Position(0, 0);
		Assert.That(pos1.Equals(pos2), Is.True);
	}

	[Test]
	public void Are_not_equal_when_not_with_the_same_Row_and_column()
	{
		var pos1 = new Position(1, 0);
		var pos2 = new Position(0, 0);
		Assert.That(pos1.Equals(pos2), Is.False);
	}

	[Test]
	public void Are_equal_when_with_the_same_row_and_column_using_the_operator()
	{
		var pos1 = new Position(0, 0);
		var pos2 = new Position(0, 0);
		Assert.That(pos1 == pos2, Is.True);
	}

	[Test]
	public void Are_not_equal_when_not_with_the_same_Row_and_column_using_the_operator()
	{
		var pos1 = new Position(1, 0);
		var pos2 = new Position(0, 0);
		Assert.That(pos1 != pos2, Is.True);
	}

	[Test]
	public void Plus_operator_adds_the_columns_and_rows()
	{
		var pos1 = new Position(1, 0);
		var pos2 = new Position(0, 1);
		var posRet = pos1 + pos2;
		Assert.AreEqual(new Position(1, 1), posRet);
	}

	[Test]
	public void Minus_operator_subtracts_the_columns_and_rows()
	{
		var pos1 = new Position(1, 0);
		var pos2 = new Position(0, 1);
		var posRet = pos1 - pos2;
		Assert.AreEqual(new Position(1, -1), posRet);
	}

	[TestCase(0, 1, 3, 0, 3)]
	public void Multiply_operator_multiplies_Position(int posCol, int posRow, int multiplier, int expectedCol, int expectedRow)
	{
		var pos = new Position(posCol, posRow) * multiplier;
		Assert.AreEqual(new Position(expectedCol, expectedRow), pos);
	}

	[Test]
	public void Plus_and_Multiply_operators_first_multiples_then_adds()
	{
		var posStart = new Position(3, 0);
		var posMult = new Position(0, 1);
		var posRet = posStart + posMult * 3;
		Assert.AreEqual(new Position(3, 3), posRet);
	}

	[TestCase(0, 0, 0, 1, 1)]
	[TestCase(0, 0, 1, 1, 1)]
	[TestCase(0, 0, 2, 0, 2)]
	[TestCase(0, 0, 2, 2, 2)]
	[TestCase(7, 7, 6, 7, 1)]
	public void Grid_distance(int posCol1, int posRow1, int posCol2, int posRow2, int expectedDistance)
	{
		Assert.That(Position.GridDistance(new Position(posCol1, posRow1), new Position(posCol2, posRow2)), Is.EqualTo(expectedDistance));
	}

	[Test]
	public void Grid_normal()
	{
		var pos1 = new Position(3, 2);
		var pos2 = new Position(3, 0);
		var posNormal = Position.GridNormal(pos1, pos2);
		Assert.AreEqual(new Position(0, -1), posNormal);
		
		pos1 = new Position(3, 3);
		pos2 = new Position(3, 0);
		posNormal = Position.GridNormal(pos1, pos2);
		Assert.AreEqual(new Position(0, -1), posNormal);
		
		pos1 = new Position(3, 2);
		pos2 = new Position(3, 2);
		posNormal = Position.GridNormal(pos1, pos2);
		Assert.AreEqual(new Position(0, 0), posNormal);
	}
}