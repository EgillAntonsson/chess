using Chess;
using NUnit.Framework;

namespace PositionTest
{
	public class Constructor
	{
		[Test]
		public void ColumnAndRow_AreInitialized()
		{
			var position = new Position(0, 1);
			Assert.That(position.Column, Is.EqualTo(0));
			Assert.That(position.Row, Is.EqualTo(1));
		}
	}
	
	public class Equals
	{
		[Test]
		public void AreEqual_WhenWithSameRowAndSameColumn()
		{
			var pos1 = new Position(0, 0);
			var pos2 = new Position(0, 0);
			Assert.That(pos1.Equals(pos2), Is.True);
		}
		
		[Test]
		public void NotEqual_WhenNotWithSameRowAndSameColumn()
		{
			var pos1 = new Position(1, 0);
			var pos2 = new Position(0, 0);
			Assert.That(pos1.Equals(pos2), Is.False);
		}
		
		[Test]
		public void AreEqual_WhenWithSameRowAndSameColumn_UsingOperator()
		{
			var pos1 = new Position(0, 0);
			var pos2 = new Position(0, 0);
			Assert.That(pos1 == pos2, Is.True);
		}
		
		[Test]
		public void AreNotEqual_WhenNotWithSameRowAndSameColumn_UsingOperator()
		{
			var pos1 = new Position(1, 0);
			var pos2 = new Position(0, 0);
			Assert.That(pos1 != pos2, Is.True);
		}
	}
}