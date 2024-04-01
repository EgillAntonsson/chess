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
		private Dictionary<Position, (TileWithCastlingPiece, Position)> castlingTileByCheckableTilePosition;
		private TileWithPiece foundMovesForTile;

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
			var player = players.First(p => p.PlayerId == PlayerIdToMove);
			var foundMoves = ChessBoard.FindMoves(tile, player);
			foundMovesForTile = tile;
			castlingTileByCheckableTilePosition = foundMoves.castlingTileByCheckableTilePosition;
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
			
			var (movedTileWithPiece, changedTiles, _) = ChessBoard.MovePiece(tile, position, castlingTileByCheckableTilePosition);
			players.First(p => p.PlayerId == tile.Piece.PlayerId).LastMovedTilePiece = movedTileWithPiece;
			castlingTileByCheckableTilePosition.Clear();

			var opponentsInCheck = players.Where(p => p.PlayerId != tile.Piece.PlayerId).
						Select(p => ChessBoard.IsPlayerInCheck(p.PlayerId, rules.ValidMovesByType));

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
					where opp.Item1.PlayerId == p.PlayerId
					select new Player(opp.Item1.PlayerId, opp.Item1.IsInCheckType)).ToList();
			pla.Add(pl.First(t => t.PlayerId == PlayerIdToMove));
			return pla;
		}
		
		private static int UpdatePlayerTurn(int pIdToMove, IEnumerable<Player> pl)
		{
			return pIdToMove == pl.Count() ? 1 : pIdToMove + 1;
		}
	}
}
