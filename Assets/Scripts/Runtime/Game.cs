
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class Game
	{
		public ChessBoard ChessBoard { get; }
		private readonly Variant variant;
		public int PlayerIdToMove { get; private set; }
		public int TurnNumber { get; private set; }

		public Game(Variant variant)
		{
			this.variant = variant;
			ChessBoard = new ChessBoard();
			PlayerIdToMove = variant.PlayerIdToStart;
			TurnNumber = 1;
		}
		
		public Tile[,] Create()
		{
			return ChessBoard.Create(variant.Tiles);
		}

		public IEnumerable<Position> FindValidMoves(TileWithPiece tile)
		{
			var isCheckableTile = tile.Piece.Type == variant.CheckablePieceType;
			return ChessBoard.FindMoves(tile, isCheckableTile, variant.ValidMovesByType, PlayerIdToMove);
		}
		
		public (Tile beforeMoveTile, Tile afterMoveTile, IEnumerable<(CheckType checktype, Tile checkTile)>, bool) MovePiece(TileWithPiece tile, Position position)
		{
			var (beforeMoveTile, afterMoveTile) = ChessBoard.MovePiece(tile, position);
			IEnumerable<(CheckType checktype, Tile checkTile)> opponentInCheckList = null;
			if (variant.CanCheck)
			{
				opponentInCheckList = GetOpponentsLeft().Select(playerId => ChessBoard.IsPlayerInCheck(playerId, variant.CheckablePieceType, variant.ValidMovesByType));
			}

			var gameHasEnded = CheckIfGameHasEnded(opponentInCheckList);
			
			PlayerTurnEnded();
			return (beforeMoveTile, afterMoveTile, opponentInCheckList, gameHasEnded);
		}
		
		private bool CheckIfGameHasEnded(IEnumerable<(CheckType checktype, Tile checkTile)> opponentInCheckList)
		{ 
			if (variant.EndConditions.Contains(EndConditionType.CheckMate))
			{
				if (opponentInCheckList != null)
				{
					return opponentInCheckList.All(opponentInCheck => opponentInCheck.checktype == CheckType.CheckMate);
				}
			}
			return false;
		}

		private IEnumerable<int> GetOpponentsLeft()
		{
			var opponents = new List<int>();
			for (var i = 1; i <= variant.NumberOfPlayers; i++)
			{
				if (i == PlayerIdToMove)
				{
					continue;
				}
				opponents.Add(i);
			}
			return opponents;
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
