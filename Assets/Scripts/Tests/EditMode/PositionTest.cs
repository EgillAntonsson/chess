using NUnit.Framework;

public class PositionTest
{
	public class Constructor
	{
		[Test]
		public void Position_IsSet()
		{
			var position = new Position(2, 3);
			Assert.That(position.Row, Is.EqualTo(2));
			Assert.That(position.Column, Is.EqualTo(3));
			Assert.That(position, Is.EqualTo(new Position(2, 3)));
		}
		
		[Test]
		public void Position_IsSet_AndCanBeComparedWithEqual()
		{
			var position = new Position(2, 3);
			Assert.That(position, Is.EqualTo(new Position(2, 3)));
		}
	}
}
