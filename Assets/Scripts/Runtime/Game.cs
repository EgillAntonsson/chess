
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class Game
	{
		public Board ChessBoard { get; }
		private readonly Variant variant;
		public int PlayerIdToMove { get; private set; }

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
		
		public TileWithPiece MovePiece(TileWithPiece tile, Position position)
		{
			PlayerIdToMove = PlayerTurnEnded(PlayerIdToMove);
			return ChessBoard.MovePiece(tile, position);
		}

		private int PlayerTurnEnded(int playerId)
		{
			return playerId == variant.NumberOfPlayers ? 1 : playerId + 1;
		}
	}
}
