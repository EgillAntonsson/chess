using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Chess.Test.EditMode")]

namespace Chess
{
	public class ChessBoard
	{
		private Tile[,] boardTiles;
		private Dictionary<Position, TileWithPiece> tileByStartPos;
		private Dictionary<int, IEnumerable<TileWithPiece>> tilesByPlayer;

		public Tile[,] Create(string tiles)
		{
			(boardTiles, tileByStartPos, tilesByPlayer) = Board.Create(tiles);
			return boardTiles;
		}

		internal (Tile[,],
			Dictionary<Position, TileWithPiece>,
			Dictionary<int, IEnumerable<TileWithPiece>>) InjectBoard(string tiles)
		{
			(boardTiles, _, tilesByPlayer) = Board.Create(tiles);
			return (boardTiles, tileByStartPos, tilesByPlayer);
		}

		public IEnumerable<Position> FindMoves(TileWithPiece tileWithPiece, bool isInCheck, int playerId, Variant rules)
		{
			var checkableTwp = GetCheckableTileWithPiece(playerId, rules.CheckablePieceType);
			var isCheckableTwp = tileWithPiece == checkableTwp;
			const MoveCaptureFlag moveCaptureFlags = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			var playerTilePieces = tilesByPlayer[playerId];
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerId);

			var movePositions = FilterAwayCheckedMovePositions(
				Board.FindMovePositions(tileWithPiece, rules.ValidMovesByType, playerId, boardTiles, tileByStartPos, moveCaptureFlags)
			);

			if (!isCheckableTwp) return movePositions;
			
			var castingPositions = FindCastlingMoves(isInCheck, playerId, rules, checkableTwp, playerTilePieces, opponentTiles);
			return movePositions.Concat(castingPositions);

			IEnumerable<Position> FilterAwayCheckedMovePositions(IEnumerable<Position> positions)
			{
				return positions.Where(pos => !Board.IsInCheckAfterMove(checkableTwp, tileWithPiece, pos, boardTiles, playerTilePieces, tileByStartPos, opponentTiles, rules.ValidMovesByType));
			}
		}

		private IEnumerable<Position> FindCastlingMoves(bool isInCheck, int playerId,
			Variant rules,
			TileWithPiece checkableTwp,
			IEnumerable<TileWithPiece> playerTilePieces,
			IEnumerable<TileWithPiece> opponentTiles)
		{
			var positions = new List<Position>();
			if (isInCheck)
			{
				return positions;
			}
			var checkableIsInStartPos = tileByStartPos.ContainsKey(checkableTwp.Position);
			var castlingTiles = tilesByPlayer[playerId].Where(twp => twp.Piece.Type == rules.CastlingPieceType);
			var castlingTilesInStartPos = castlingTiles.Where(ct => tileByStartPos.ContainsKey(ct.Position));
			
			var tilesInStartPositions = castlingTilesInStartPos as TileWithPiece[] ?? castlingTilesInStartPos.ToArray();
			if (!checkableIsInStartPos || !tilesInStartPositions.Any())
			{
				return positions;
			}

			foreach (var ctsp in tilesInStartPositions)
			{
				var nrOfColumnWithDirection = ctsp.Position.Column - checkableTwp.Position.Column;
				var sign = Math.Sign(nrOfColumnWithDirection);
				var nrOfColumn = Math.Abs(nrOfColumnWithDirection);
				var isEmpty = true;
				for (var i = 1; i < nrOfColumn; i++)
				{
					isEmpty = Board.GetTile(new Position(checkableTwp.Position.Column + i * sign, checkableTwp.Position.Row), boardTiles) is not TileWithPiece;
					if (!isEmpty) break;
				}

				if (!isEmpty) continue;

				var columnsMoves = new[] { checkableTwp.Position.Column + 1 * sign, checkableTwp.Position.Column + 2 * sign };
				if (columnsMoves.Select(colToCheck => Board.IsInCheckAfterMove(checkableTwp,
						checkableTwp,
						new Position(colToCheck, checkableTwp.Position.Row),
						boardTiles,
						playerTilePieces,
						tileByStartPos,
						opponentTiles,
						rules.ValidMovesByType))
					.Any(isInCheckAfterMove => isInCheckAfterMove))
				{
					return positions;
				}

				positions.Add(new Position(checkableTwp.Position.Column + 2 * sign, checkableTwp.Position.Row));
			}

			return positions;
		}

		public static IEnumerable<TileWithPiece> GetOpponentTiles(Dictionary<int, IEnumerable<TileWithPiece>> tbp, int playerId)
		{
			return tbp.Where(kvp => kvp.Key != playerId).SelectMany(kvp => kvp.Value);
		}

		public (Tile beforeMoveTile, TileWithPiece afterMoveTile) MovePiece(TileWithPiece twp, Position pos)
		{
			var playerId = twp.Piece.PlayerId;
			var ret = Board.MovePiece(twp, pos, boardTiles, tilesByPlayer[playerId]);
			boardTiles = ret.tilesAfterMove;
			tilesByPlayer[playerId] = ret.playerTilePiecesAfterMove;

			return (ret.beforeMoveTile, ret.afterMoveTile);
		}

		public (int playerId, CheckType checktype, Tile checkTile) IsPlayerInCheck(int playerId,
			PieceType checkablePieceType,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc)
		{
			var checkablePieceTile = GetCheckableTileWithPiece(playerId, checkablePieceType);
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerId);
			var checkType = Board.IsInCheck(checkablePieceTile, movesForPieceTypeFunc, opponentTiles, boardTiles, tileByStartPos, tilesByPlayer[playerId]);
			return (playerId, checkType, checkablePieceTile);
		}

		private TileWithPiece GetCheckableTileWithPiece(int playerId, PieceType checkablePieceType)
		{
			return tilesByPlayer[playerId].First(twp => twp.Piece.Type == checkablePieceType);
		}
	}
}