using System.Linq;

namespace Chess
{
	public class ChessBoard
	{
		public const int BoardSize = 8;
		private Board board;

		public ChessBoard(Variant variant)
		{
			board = new Board(variant.TileSetupSequence);
		}
		
		public bool HasGameEnded(Variant variant)
		{
			return variant.EndConditions.Any(endCondition => endCondition(this));
		}
	}
}