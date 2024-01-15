
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
		private Position moveAdditionPosLastPlayed;

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
			return ChessBoard.FindValidMoves(tile, variant.ValidMovesByType, PlayerIdToMove, moveAdditionPosLastPlayed);
		}
		
		public (Tile beforeMoveTile, Tile afterMoveTile) MovePiece(TileWithPiece tile, Position position)
		{
			var (beforeMoveTile, afterMoveTile, isFirstMoveAddition) = ChessBoard.MovePiece(tile, position, variant.ValidMovesByType, PlayerIdToMove);
			PlayerTurnEnded();
			moveAdditionPosLastPlayed = isFirstMoveAddition ? afterMoveTile.Position : Position.None;
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
