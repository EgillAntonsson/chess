using NUnit.Framework;

public class KingTest
{
	public class Constructor
	{
		[Test]
		public void HasInitialPosition()
		{
			const int row = 0;
			const int column = 0;
			var king = new King(row, column);
			Assert.That(king.Position.Row, Is.EqualTo(row));
			Assert.That(king.Position.Column, Is.EqualTo(column));
		}
	}
}
