
namespace Chess
{
	public class Game
	{
		private readonly ChessBoard chessBoard;
		private readonly Variant variant;

		public Game(Variant variant)
		{
			chessBoard = new ChessBoard(variant);
			
		}
		
		public bool HasGameEnded()
		{
			return chessBoard.HasGameEnded(variant);
		}
	}
}
