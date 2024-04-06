using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess
{
	public class Game
	{
		private readonly Rules rules;
		private IEnumerable<Player> players;
		private ChessBoard ChessBoard { get; }
		public int PlayerIdToMove { get; private set; }
		public int TurnNumber { get; private set; }
		private TileWithPiece foundMovesForTile;
		private Dictionary<Position, (TileWithCastlingPiece, Position)> castlingTileByCheckableTilePosition;
		private IEnumerable<(Position, TileWithPiece)> pairsOfInPassingCapturePosAndPassedPiece;

		public Game(Rules rules)
		{
			this.rules = rules;
			ChessBoard = new ChessBoard(rules);
			PlayerIdToMove = rules.PlayerIdToStart;
			TurnNumber = 1;

			players = new List<Player>();
			for (var i = 1; i <= rules.NumberOfPlayers; i++)
			{
				players = players.Append(new Player(i, CheckType.NoCheck));
			}
		}
		
		public Tile[,] Create()
		{
			var (tiles, tbsp, tbp) = ChessBoard.Create(rules.BoardAtStart);
			var (success, errorMessage) = ChessBoard.ValidateBoard(tiles, tbsp, tbp);
			if (!success)
			{
				throw new ApplicationException(errorMessage);
			}
			return tiles;
		}

		public IEnumerable<Position> FindValidMoves(TileWithPiece tile)
		{
			var player = players.First(p => p.Id == PlayerIdToMove);
			var lastMoveOfOpponents = players
				.Where(p => p.Id != PlayerIdToMove)
				.Select(p => p.LastMovedTilePiece)
				.Where(twp => twp != null);
			var foundMoves = ChessBoard.FindMoves(tile, player, lastMoveOfOpponents);
			foundMovesForTile = tile;
			castlingTileByCheckableTilePosition = foundMoves.castlingTileByCheckableTilePosition;
			pairsOfInPassingCapturePosAndPassedPiece = foundMoves.pairsOfInPassingCapturePosAndPassedPiece;
			return foundMoves.movePositions;
		}
		
		public (IEnumerable<Tile> changedTiles, IEnumerable<(Player, Tile checkTile)>, bool) MovePiece(TileWithPiece tile, Position position)
		{
			// TODO: write a test and then refactor the fail handling.
			if (PlayerIdToMove != tile.Piece.PlayerId)
			{
				Debug.LogError("This is not the player turn to move.");
			}
			if (tile != foundMovesForTile)
			{
				var foundMovePositions = FindValidMoves(tile);
				if (foundMovePositions.All(p => p != position))
				{
					Debug.LogError("move position for tile piece is not a valid move position");
				}
			}
			
			var (movedTileWithPiece, changedTiles, _) = ChessBoard.MovePiece(tile, position, castlingTileByCheckableTilePosition, pairsOfInPassingCapturePosAndPassedPiece);
			players.First(p => p.Id == tile.Piece.PlayerId).LastMovedTilePiece = movedTileWithPiece;
			castlingTileByCheckableTilePosition.Clear();

			var opponentsInCheck = players.Where(p => p.Id != tile.Piece.PlayerId).
						Select(p => ChessBoard.IsPlayerInCheck(p.Id, rules.MovesByType));

			var gameHasEnded = CheckIfGameHasEnded(opponentsInCheck, rules);

			players = UpdatePlayers(players, opponentsInCheck);
			PlayerIdToMove = UpdatePlayerTurn(PlayerIdToMove, players);

			return (changedTiles, opponentsInCheck, gameHasEnded);
		}

		private static bool CheckIfGameHasEnded(IEnumerable<(Player, Tile checkTile)> opponentsInCheck, Rules rules)
		{
			return rules.EndConditions.Contains(EndConditionType.CheckMate)
					&& opponentsInCheck.All(oppInCheck => oppInCheck.Item1.IsInCheckType == CheckType.CheckMate);
		}

		private IEnumerable<Player> UpdatePlayers(IEnumerable<Player> pl, IEnumerable<(Player, Tile)> opponents)
		{
			var pla = (from p in pl from opp in opponents
					where opp.Item1.Id == p.Id
					select new Player(opp.Item1.Id, opp.Item1.IsInCheckType)).ToList();
			pla.Add(pl.First(t => t.Id == PlayerIdToMove));
			return pla;
		}
		
		private static int UpdatePlayerTurn(int pIdToMove, IEnumerable<Player> pl)
		{
			return pIdToMove == pl.Count() ? 1 : pIdToMove + 1;
		}
	}
}
