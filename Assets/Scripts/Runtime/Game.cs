
using System.Collections.Generic;

namespace Chess
{
	public class Game
	{
		public ChessBoard ChessBoard { get; }
		private readonly Variant variant;
		public int PlayerIdToMove { get; set; }

		public Game(Variant variant)
		{
			this.variant = variant;
			ChessBoard = new ChessBoard(variant);
			PlayerIdToMove = variant.PlayerIdToStart;
		}

		public bool HasGameEnded()
		{
			return ChessBoard.HasGameEnded(variant);
		}

		public IEnumerable<Tile> ShowValidMoves(Tile tile)
		{
			return null;
			// return ChessBoard.ShowValidMoves(tile);
		}
	}
}
