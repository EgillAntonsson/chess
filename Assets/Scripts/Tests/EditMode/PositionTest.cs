using System;
using NUnit.Framework;

public abstract class PositionTest
{
	public class Constructor
	{
		[Test]
		public void RowAndColumn_AreInitialized()
		{
			var position = new Position(0, 1);
			Assert.That(position.Row, Is.EqualTo(0));
			Assert.That(position.Column, Is.EqualTo(1));
		}

		[Test]
		public void Error_WhenRowParamIsInvalid()
		{
			var exception = Assert.Throws(Is.TypeOf<ArgumentOutOfRangeException>(), delegate
			{
				new Position(-1, 0);
			});
			Assert.That((exception as ArgumentException)?.ParamName, Does.Match("coord").IgnoreCase);
			Assert.That(exception.Message, Does.Match("invalid").IgnoreCase);
		}

		[Test]
		public void Error_WhenColumnParamIsInvalid()
		{
			var exception = Assert.Throws(Is.TypeOf<ArgumentOutOfRangeException>(), delegate
			{
				new Position(0, -1);
			});
			Assert.That((exception as ArgumentException)?.ParamName, Does.Match("coord").IgnoreCase);
			Assert.That(exception.Message, Does.Match("invalid").IgnoreCase);
		}
	}
}