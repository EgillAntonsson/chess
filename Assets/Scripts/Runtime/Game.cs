
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess
{
	public class Game
	{
		private readonly Variant variant;
		private IEnumerable<(int, CheckType)> players; 
		public ChessBoard ChessBoard { get; }
		public int PlayerIdToMove { get; private set; }
		public int TurnNumber { get; private set; }

		public Game(Variant variant)
		{
			this.variant = variant;
			ChessBoard = new ChessBoard(variant);
			PlayerIdToMove = variant.PlayerIdToStart;
			TurnNumber = 1;

			players = new List<(int, CheckType)>();
			for (var i = 1; i <= variant.NumberOfPlayers; i++)
			{
				players = players.Append((i, CheckType.NoCheck));
			}
		}
		
		public Tile[,] Create()
		{
			var (tiles, _, _) = ChessBoard.Create(variant.Tiles);
			return tiles;
		}

		public IEnumerable<Position> FindValidMoves(TileWithPiece tile)
		{
			var player = players.First(p => p.Item1 == PlayerIdToMove);
			var isInCheck = player.Item2 == CheckType.Check;
			return ChessBoard.FindMoves(tile, isInCheck, PlayerIdToMove, variant);
		}
		
		public (Tile beforeMoveTile, Tile afterMoveTile, IEnumerable<(int playerId, CheckType checktype, Tile checkTile)>, bool) MovePiece(TileWithPiece tile, Position position)
		{
			var (beforeMoveTile, afterMoveTile) = ChessBoard.MovePiece(tile, position);
			var opponents = Enumerable.Empty<(int, CheckType, Tile)>();
			if (variant.CanCheck)
			{
				opponents = players.Where(tuple => tuple.Item1 != tile.Piece.PlayerId).
						Select(tuple => ChessBoard.IsPlayerInCheck(tuple.Item1, StandardVariant.CheckablePieceTypeStandard, variant.ValidMovesByType));
			}

			var gameHasEnded = CheckIfGameHasEnded(opponents, variant);

			players = UpdatePlayers(PlayerIdToMove, players, opponents);
			PlayerIdToMove = UpdatePlayerTurn(PlayerIdToMove, players);
			Debug.Log(PlayerIdToMove);
			return (beforeMoveTile, afterMoveTile, opponents, gameHasEnded);
		}
		
		private bool CheckIfGameHasEnded(IEnumerable<(int playerId, CheckType checktype, Tile checkTile)> opponentInCheckList, Variant v)
		{
			return v.EndConditions.Contains(EndConditionType.CheckMate)
					&& opponentInCheckList.All(opponentInCheck => opponentInCheck.checktype == CheckType.CheckMate);
		}

		private IEnumerable<(int, CheckType)> UpdatePlayers(int pIdToMove, IEnumerable<(int, CheckType)> pl, IEnumerable<(int, CheckType, Tile)> opponents)
		{
			var pla = (from p in pl from opp in opponents
					where opp.Item1 == p.Item1
					select new ValueTuple<int, CheckType>(opp.Item1, opp.Item2)).ToList();
			pla.Add(pl.First(t => t.Item1 == PlayerIdToMove));
			return pla;
		}
		
		private static int UpdatePlayerTurn(int pIdToMove, IEnumerable<(int, CheckType)> pl)
		{
			return pIdToMove == pl.Count() ? 1 : pIdToMove + 1;
		}
	}
}
