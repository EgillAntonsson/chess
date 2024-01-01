using System.Linq;

namespace Chess
{
	public class ChessBoard
	{
		public Board Board { get;  }

		public ChessBoard(Variant variant)
		{
			Board = new Board(variant.TileSetupSequence);
		}
		
		public bool HasGameEnded(Variant variant)
		{
			return variant.EndConditions.Any(endCondition => endCondition(this));
		}
	}
}