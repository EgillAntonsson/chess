
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Chess
{
	public class Game
	{
		private readonly Rules rules;
		private IEnumerable<(int, CheckType)> players; 
		public ChessBoard ChessBoard { get; }
		public int PlayerIdToMove { get; private set; }
		public int TurnNumber { get; private set; }

		public Game(Rules rules)
		{
			this.rules = rules;
			ChessBoard = new ChessBoard(rules);
			PlayerIdToMove = rules.PlayerIdToStart;
			TurnNumber = 1;

			players = new List<(int, CheckType)>();
			for (var i = 1; i <= rules.NumberOfPlayers; i++)
			{
				players = players.Append((i, CheckType.NoCheck));
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
			var player = players.First(p => p.Item1 == PlayerIdToMove);
			var isInCheck = player.Item2 == CheckType.Check;
			return ChessBoard.FindMoves(tile, isInCheck, PlayerIdToMove);
		}
		
		public (IEnumerable<Tile> changedTiles, IEnumerable<(int playerId, CheckType checktype, Tile checkTile)>, bool) MovePiece(TileWithPiece tile, Position position)
		{
			var (changedTiles, _) = ChessBoard.MovePiece(tile, position);
			var opponentsInCheck = players.Where(tuple => tuple.Item1 != tile.Piece.PlayerId).
						Select(tuple => ChessBoard.IsPlayerInCheck(tuple.Item1, StandardRules.CheckablePieceTypeStandard, rules.ValidMovesByType));

			var gameHasEnded = CheckIfGameHasEnded(opponentsInCheck, rules);

			players = UpdatePlayers(PlayerIdToMove, players, opponentsInCheck);
			PlayerIdToMove = UpdatePlayerTurn(PlayerIdToMove, players);

			return (changedTiles, opponentsInCheck, gameHasEnded);
		}
		
		private bool CheckIfGameHasEnded(IEnumerable<(int playerId, CheckType checktype, Tile checkTile)> opponentsInCheck, Rules rules)
		{
			return rules.EndConditions.Contains(EndConditionType.CheckMate)
					&& opponentsInCheck.All(oppInCheck => oppInCheck.checktype == CheckType.CheckMate);
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
