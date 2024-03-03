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
			(boardTiles, tileByStartPos, tilesByPlayer) = Board.Create(tiles);
			return (boardTiles, tileByStartPos, tilesByPlayer);
		}

		public IEnumerable<Position> FindMoves(TileWithPiece tileWithPiece, bool isInCheck, int playerId, Variant rules)
		{
			var checkableTwp = GetCheckableTileWithPiece(playerId, rules.CheckablePieceType);
			var isCheckableTwp = tileWithPiece == checkableTwp;
			const MoveCaptureFlag moveCaptureFlags = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			var playerTilePieces = tilesByPlayer[playerId];
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerId);

			var movePositions = Board.FindMovePositions(tileWithPiece, rules.ValidMovesByType, playerId, boardTiles, tileByStartPos, moveCaptureFlags);

			if (isCheckableTwp && !isInCheck)
			{
				var checkableIsInStartPos = tileByStartPos.ContainsKey(checkableTwp.Position);
				var castlingTiles = tilesByPlayer[playerId].Where(twp => twp.Piece.Type == rules.CastlingPieceType);
				var castlingTilesInStartPos = castlingTiles.Where(ct => tileByStartPos.ContainsKey(ct.Position));
				if (checkableIsInStartPos && castlingTilesInStartPos.Any())
				{
					foreach (var ctsp in castlingTilesInStartPos)
					{
						var nrOfColumnWithDirection = ctsp.Position.Column - tileWithPiece.Position.Column;
						var sign = Math.Sign(nrOfColumnWithDirection);
						var nrOfColumn = Math.Abs(nrOfColumnWithDirection);
						var isEmpty = true;
						for (var i = 1; i < nrOfColumn; i++)
						{
							isEmpty = Board.GetTile(new Position(checkableTwp.Position.Column + i * sign, checkableTwp.Position.Row), boardTiles) is not TileWithPiece;
						}

						if (!isEmpty)
						{
							continue;
						}

						var isInCheckAfterFirstMove = Board.IsInCheckAfterMove(checkableTwp,
							checkableTwp,
							new Position(checkableTwp.Position.Column + 1, checkableTwp.Position.Row),
							boardTiles,
							playerTilePieces,
							tileByStartPos,
							opponentTiles,
							rules.ValidMovesByType);
						if (isInCheckAfterFirstMove)
						{
							continue;
						}

						var isInCheckAfterSecondMove = Board.IsInCheckAfterMove(checkableTwp,
							checkableTwp,
							new Position(checkableTwp.Position.Column + 2, checkableTwp.Position.Row),
							boardTiles,
							playerTilePieces,
							tileByStartPos,
							opponentTiles,
							rules.ValidMovesByType);

						if (isInCheckAfterSecondMove)
						{
							continue;
						}
						movePositions = movePositions.Append(new Position(checkableTwp.Position.Column + 2, checkableTwp.Position.Row));

					}
				}

			}

			return !isCheckableTwp && !isInCheck ? movePositions : IsInCheckAfterMoveSimulation(movePositions);

			IEnumerable<Position> IsInCheckAfterMoveSimulation(IEnumerable<Position> positions)
			{
				return positions.Where(pos => !Board.IsInCheckAfterMove(checkableTwp, tileWithPiece, pos, boardTiles, playerTilePieces, tileByStartPos, opponentTiles, rules.ValidMovesByType));
			}
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