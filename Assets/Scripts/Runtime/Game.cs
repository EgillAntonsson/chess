
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
			return ChessBoard.FindValidMoves(tile, variant.ValidMovesByType, PlayerIdToMove);
		}
		
		public (Tile beforeMoveTile, Tile afterMoveTile, IEnumerable<(bool isCheck, bool isCheckMate, Tile checkTile)>, bool) MovePiece(TileWithPiece tile, Position position)
		{
			var (beforeMoveTile, afterMoveTile) = ChessBoard.MovePiece(tile, position);
			IEnumerable<(bool isCheck, bool isCheckMate, Tile checkTile)> checkTuples = null;
			if (variant.CanCheck)
			{
				checkTuples = GetOpponentsLeft().Select(playerId => ChessBoard.IsCheck(playerId));
			}

			var gameHasEnded = CheckIfGameHasEnded(checkTuples);
			
			PlayerTurnEnded();
			return (beforeMoveTile, afterMoveTile, checkTuples, gameHasEnded);
		}
		
		private bool CheckIfGameHasEnded(IEnumerable<(bool isCheck, bool isCheckMate, Tile checkTile)> checkTuples)
		{ 
			if (variant.EndConditions.Contains(EndConditionType.CheckMate))
			{
				if (checkTuples != null)
				{
					return checkTuples.All(tuple => tuple.isCheckMate);
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
