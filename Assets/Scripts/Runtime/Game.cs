using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess
{
	public class Game
	{
		private readonly Rules rules;
		private IEnumerable<Player> players;
		private ChessBoard ChessBoard { get; }
		public int PlayerIdToMove { get; private set; }
		private TileWithPiece foundMovesForTile;
		private Dictionary<Position, (TileWithCastlingPiece, Position)> castlingTileByCheckableTilePosition;
		private IEnumerable<(Position, TileWithPiece)> pairsOfInPassingCapturePosAndPassedPiece;
		public bool GameHasEnded { get; private set; }
		public bool PromotionIsOccuring { get; private set; }

		public Game(Rules rules, ChessBoard chessBoard)
		{
			this.rules = rules;
			ChessBoard = chessBoard;
			PlayerIdToMove = rules.PlayerIdToStart;

			players = new List<Player>();
			for (var i = 1; i <= rules.NumberOfPlayers; i++)
			{
				players = players.Append(new Player(i));
			}
		}
		
		public Tile[,] Create()
		{
			var (tiles, tbsp, tbp) = ChessBoard.Create(rules.BoardAtStart);
			return tiles;
		}

		public IEnumerable<Position> FindMovePositions(TileWithPiece tile)
		{
			if (GameHasEnded || PromotionIsOccuring)
			{
				return Enumerable.Empty<Position>();
			}
			var player = players.First(p => p.Id == tile.Piece.PlayerId);
			var lastMoveOfOpponents = players
				.Where(p => p.Id != PlayerIdToMove)
				.Select(p => p.LastMovedTilePiece)
				.Where(twp => twp != null);
			var foundMoves = ChessBoard.FindMoves(tile, player.IsInCheckType, lastMoveOfOpponents);
			foundMovesForTile = tile;
			castlingTileByCheckableTilePosition = foundMoves.castlingTileByCheckableTilePosition;
			pairsOfInPassingCapturePosAndPassedPiece = foundMoves.pairsOfInPassingCapturePosAndPassedPiece;
			return foundMoves.movePositions;
		}
		
		public async Task<(IEnumerable<Tile> changedTiles, IEnumerable<(Player player, Position checkTilePos)> playersWithCheckTilePos, Dictionary<int, Result> playersEndResult)>
			MovePiece(TileWithPiece tile, Position position, Func<Task<PieceType>> promoteAsync)
		{
			var retWhenInvalid = (Enumerable.Empty<Tile>(), Enumerable.Empty<(Player player, Position checkTilePos)>(), new Dictionary<int, Result>());
			if (GameHasEnded || PromotionIsOccuring)
			{
				return retWhenInvalid;
			}
			var playerIdThatIsMoving = tile.Piece.PlayerId;
			if (PlayerIdToMove != playerIdThatIsMoving)
			{
				// could return the reason "Not the turn to move for this player."
				return retWhenInvalid;
			}
			if (tile != foundMovesForTile)
			{
				var foundMovePositions = FindMovePositions(tile);
				if (foundMovePositions.All(p => p != position))
				{
					// could return the reason ""Move position for tile piece is not a valid move position"
					return retWhenInvalid;
				}
			}
			
			if (ShouldPromotionOccur(tile, rules))
			{
				PromotionIsOccuring = true;
				var promotedType = await promoteAsync();
				tile = ChessBoard.PromotePiece(tile, promotedType);
				PromotionIsOccuring = false;
			}

			var (movedTile, changedTiles, tiles) = ChessBoard.MovePiece(tile, position, castlingTileByCheckableTilePosition, pairsOfInPassingCapturePosAndPassedPiece);

			players.First(p => p.Id == playerIdThatIsMoving).LastMovedTilePiece = movedTile;
			
			// Process End Of Move
			var playersWithCheckTilePos = players.Select(p => ChessBoard.IsPlayerInCheck(p.Id, rules.MoveDefinitionByType));
			players = playersWithCheckTilePos.Select(p => p.player);

			var endConditionsResult = CheckEndConditions(playerIdThatIsMoving, players, rules.EndConditions, tiles);
			if (endConditionsResult.gameHasEnded)
			{
				GameHasEnded = true;
			}
			else
			{
				PlayerIdToMove = UpdatePlayerTurn(PlayerIdToMove, players);
			}

			return (changedTiles, playersWithCheckTilePos, endConditionsResult.Item2);
		}

		public static bool ShouldPromotionOccur(TileWithPiece twp, Rules rules)
		{
			if (twp.Piece.Type != rules.PromotionPieceType)
			{
				return false;
			}

			var promotionPos = rules.PromotionPosition(twp.Piece.PlayerId);
			return promotionPos.Axis switch
			{
				Position.Axis.Column => twp.Position.Column - twp.StartPosition.Column == promotionPos.AxisPositionToTravel,
				Position.Axis.Row => twp.Position.Row - twp.StartPosition.Row == promotionPos.AxisPositionToTravel,
				_ => false
			};
		}

		public (bool gameHasEnded, Dictionary<int, Result> resultByPlayerId) CheckEndConditions(int playerIdThatIsMoving,
			IEnumerable<Player> players,
			HashSet<EndCondition> endConditions,
			Tile[,] tiles)
		{
			var playerEndResults = new Dictionary<int, Result>();
			var opponents = players.Where(p => p.Id != playerIdThatIsMoving);
			if (endConditions.Select(ec => ec.Type).Contains(EndConditionType.CheckMate)
				&& opponents.All(oppInCheck => oppInCheck.IsInCheckType == CheckType.CheckMate))
			{
				var ec = endConditions.First(ec => ec.Type == EndConditionType.CheckMate);
				foreach (var opp in opponents)
				{
					playerEndResults.Add(opp.Id, ec.GetOpponentsResult());
				}
				playerEndResults.Add(playerIdThatIsMoving, ec.PlayerThatMovedResult);
				return (true, playerEndResults);
			}
			
			if (endConditions.Select(ec => ec.Type).Contains(EndConditionType.StaleMate))
			{
				var wouldBeNextPlayer = UpdatePlayerTurn(playerIdThatIsMoving, players);
				var playerPieces = Board.GetPlayerPieces(tiles, wouldBeNextPlayer);
				var movesPositions = new List<Position>();
				foreach (var pp in playerPieces)
				{
					movesPositions.AddRange(FindMovePositions(pp));
				}

				if (movesPositions.Count() == 0)
				{
					var ec = endConditions.First(ec => ec.Type == EndConditionType.StaleMate);
					foreach (var opp in opponents)
					{
						playerEndResults.Add(opp.Id, ec.GetOpponentsResult());
					}
					playerEndResults.Add(playerIdThatIsMoving, ec.PlayerThatMovedResult);
					return (true, playerEndResults);
				}
			}

			return (false, playerEndResults);
		}

		public static int UpdatePlayerTurn(int playerIdToMove, IEnumerable<Player> players)
		{
			var notMatePlayers = players.Where(p => p.IsInCheckType != CheckType.CheckMate).ToList();
			var currentIndex = notMatePlayers.FindIndex(p => p.Id == playerIdToMove);
			var nextIndex = (currentIndex + 1) % notMatePlayers.Count;
			return notMatePlayers[nextIndex].Id;
		}
	}
}
