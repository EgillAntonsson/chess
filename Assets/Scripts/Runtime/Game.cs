
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class Game
	{
		public Board ChessBoard { get; }
		private readonly Variant variant;
		public int PlayerIdToMove { get; private set; }
		public int TurnNumber { get; private set; }

		public Game(Variant variant)
		{
			this.variant = variant;
			ChessBoard = new Board(variant.Tiles);
			PlayerIdToMove = variant.PlayerIdToStart;
			TurnNumber = 1;
		}

		public bool HasGameEnded()
		{
			return variant.EndConditions.Any(endCondition => endCondition(ChessBoard));
		}

		public IEnumerable<Position> FindValidMoves(TileWithPiece tile)
		{
			return ChessBoard.FindValidMoves(tile, variant.ValidMovesByType, PlayerIdToMove);
		}
		
		public (Tile beforeMoveTile, Tile afterMoveTile) MovePiece(TileWithPiece tile, Position position)
		{
			var (beforeMoveTile, afterMoveTile) = ChessBoard.MovePiece(tile, position);
			PlayerTurnEnded();
			return (beforeMoveTile, afterMoveTile);
		}

		private void PlayerTurnEnded()
		{
			PlayerIdToMove = PlayerIdToMove == variant.NumberOfPlayers ? 1 : PlayerIdToMove + 1;
			if (PlayerIdToMove == variant.PlayerIdToStart)
			{
				TurnNumber++;
			}
		}
	}
}
