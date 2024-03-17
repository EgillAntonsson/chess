using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Chess.Test.EditMode")]

namespace Chess
{
	public class ChessBoard
	{
		private readonly Rules rules;
		private Tile[,] boardTiles;
		private Dictionary<Position, TileWithPiece> tileByStartPos;
		private Dictionary<int, IEnumerable<TileWithPiece>> tilesByPlayer;

		public ChessBoard(Rules rules)
		{
			this.rules = rules;
		}
		
		public (Tile[,], Dictionary<Position, TileWithPiece>, Dictionary<int, IEnumerable<TileWithPiece>>) Create(string tiles)
		{
			(boardTiles, tileByStartPos, tilesByPlayer) = Board.Create(tiles, rules.CheckablePieceType, rules.CastlingPieceType);
			return (boardTiles, tileByStartPos, tilesByPlayer);
		}

		public static (bool success, string errorMessage) ValidateBoard(Tile[,] tiles, Dictionary<Position, TileWithPiece> tileByStartPos, Dictionary<int, IEnumerable<TileWithPiece>> tilesByPlayer)
		{
			return (true, "");
		}

		internal (Tile[,],
			Dictionary<int, IEnumerable<TileWithPiece>>) Create_ButNotUpdateStartPos(string tiles)
		{
			(boardTiles, _, tilesByPlayer) = Create(tiles);
			return (boardTiles, tilesByPlayer);
		}

		public IEnumerable<Position> FindMoves(TileWithPiece tileWithPiece, bool isInCheck, int playerId)
		{
			const MoveCaptureFlag moveCaptureFlags = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			var playerTilePieces = tilesByPlayer[playerId];
			var playerTilePs = playerTilePieces as TileWithPiece[] ?? playerTilePieces.ToArray();
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerId);
			var oppTilePs = opponentTiles as TileWithPiece[] ?? opponentTiles.ToArray();

			var movePositions = FilterAwayCheckedMovePositions(
				Board.FindMovePositions(tileWithPiece, rules.ValidMovesByType, playerId, boardTiles, tileByStartPos, moveCaptureFlags)
			);

			if (tileWithPiece is not TileWithCheckablePiece checkableTwp) return movePositions;

			if (isInCheck || checkableTwp.HasMoved)
			{
				return movePositions;
			}

			var castingPositions = FindCastlingMoves(checkableTwp, playerTilePs, oppTilePs);
			return movePositions.Concat(castingPositions);

			IEnumerable<Position> FilterAwayCheckedMovePositions(IEnumerable<Position> positions)
			{
				return positions.Where(pos => !Board.IsInCheckAfterMove(GetCheckableTileWithPiece(playerId), tileWithPiece, pos, boardTiles, playerTilePs, tileByStartPos, oppTilePs, rules.ValidMovesByType));
			}
		}

		private IEnumerable<Position> FindCastlingMoves(TileWithPiece checkableTwp,
			TileWithPiece[] playerTilePieces,
			IEnumerable<TileWithPiece> opponentTiles)
		{
			var positions = new List<Position>();
			var castlingTilesExcludingCheckable = playerTilePieces
				.Select(c => c as TileWithCastlingPiece)
				.Where(c => c != null)
				.Where(c => c.GetType() == typeof(TileWithCastlingPiece))
				.Where(t => t.HasMoved == false);
			foreach (var castlingTiles in castlingTilesExcludingCheckable)
			{
				var nrOfColumnWithDirection = castlingTiles.Position.Column - checkableTwp.Position.Column;
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

		public (Tile beforeMoveTile, TileWithPiece afterMoveTile, Tile[,] tiles) MovePiece(TileWithPiece twp, Position pos)
		{
			var playerId = twp.Piece.PlayerId;
			var moveOutput = Board.MovePiece(twp, pos, boardTiles, tilesByPlayer[playerId]);
			boardTiles = moveOutput.tilesAfterMove;
			tilesByPlayer[playerId] = moveOutput.playerTilePiecesAfterMove;
			// if moveOutput.afterMoveTile is TileWithCheckablePiece
			return (moveOutput.beforeMoveTile, moveOutput.afterMoveTile, boardTiles);
		}

		public (int playerId, CheckType checktype, Tile checkTile) IsPlayerInCheck(int playerId,
			PieceType checkablePieceType,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc)
		{
			var checkablePieceTile = GetCheckableTileWithPiece(playerId);
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerId);
			var checkType = Board.IsInCheck(checkablePieceTile, movesForPieceTypeFunc, opponentTiles, boardTiles, tileByStartPos, tilesByPlayer[playerId]);
			return (playerId, checkType, checkablePieceTile);
		}

		private TileWithCheckablePiece GetCheckableTileWithPiece(int playerId)
		{
			return (TileWithCheckablePiece)tilesByPlayer[playerId].First(twp => twp is TileWithCheckablePiece);
		}
	}
}