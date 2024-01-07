
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class Game
	{
		public Board ChessBoard { get; }
		private readonly Variant variant;
		public int PlayerIdToMove { get; set; }

		public Game(Variant variant)
		{
			this.variant = variant;
			ChessBoard = new Board(variant.TileSetupSequence);
			PlayerIdToMove = variant.PlayerIdToStart;
		}

		public bool HasGameEnded()
		{
			return variant.EndConditions.Any(endCondition => endCondition(ChessBoard));
		}

		public IEnumerable<Position> FindValidMoves(TileWithPiece tile)
		{
			return ChessBoard.FindValidMoves(tile, variant.ValidMovesByType, PlayerIdToMove);
		}
	}
}
