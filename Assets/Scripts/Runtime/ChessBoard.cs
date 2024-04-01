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
			// TODO: implement or remove.
			return (true, "");
		}

		internal (Tile[,],
			Dictionary<int, IEnumerable<TileWithPiece>>) Create_ButNotUpdateStartPos(string tiles)
		{
			(boardTiles, _, tilesByPlayer) = Create(tiles);
			return (boardTiles, tilesByPlayer);
		}

		public (IEnumerable<Position> movePositions, Dictionary<Position, (TileWithCastlingPiece, Position)> castlingTileByCheckableTilePosition) FindMoves(TileWithPiece tileWithPiece, bool isInCheck, int playerId)
		{
			const MoveCaptureFlag moveCaptureFlags = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			var playerTilePieces = tilesByPlayer[playerId];
			var playerTilePs = playerTilePieces as TileWithPiece[] ?? playerTilePieces.ToArray();
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerId);
			var oppTilePs = opponentTiles as TileWithPiece[] ?? opponentTiles.ToArray();

			var movePositions = FilterAwayCheckedMovePositions(
				Board.FindMovePositions(tileWithPiece, rules.ValidMovesByType, playerId, boardTiles, tileByStartPos, moveCaptureFlags)
			);

			var castlingTileByCheckableTilePosition = new Dictionary<Position, (TileWithCastlingPiece, Position)>();
			if (tileWithPiece is not TileWithCheckablePiece checkableTwp) return (movePositions, castlingTileByCheckableTilePosition);
			if (isInCheck || checkableTwp.HasMoved)
			{
				return (movePositions, castlingTileByCheckableTilePosition);
			}

			castlingTileByCheckableTilePosition = FindCastlingMoves(checkableTwp, playerTilePs, oppTilePs);
			return (movePositions.Concat(castlingTileByCheckableTilePosition.Keys), castlingTileByCheckableTilePosition);

			IEnumerable<Position> FilterAwayCheckedMovePositions(IEnumerable<Position> positions)
			{
				return positions.Where(pos => !Board.IsInCheckAfterMove(GetCheckableTileWithPiece(playerId), tileWithPiece, pos, boardTiles, playerTilePs, tileByStartPos, oppTilePs, rules.ValidMovesByType));
			}
		}

		private Dictionary<Position, (TileWithCastlingPiece, Position)> FindCastlingMoves(TileWithPiece checkableTwp,
			TileWithPiece[] playerTilePieces,
			IEnumerable<TileWithPiece> opponentTiles)
		{
			var castlingTileByCheckableTilePos = new Dictionary<Position, (TileWithCastlingPiece, Position)>();
			var castlingTilesExcludingCheckable = playerTilePieces
				.Select(c => c as TileWithCastlingPiece)
				.Where(c => c != null)
				.Where(c => c.GetType() == typeof(TileWithCastlingPiece))
				.Where(t => t.HasMoved == false);
			foreach (var castlingTile in castlingTilesExcludingCheckable)
			{
				var nrOfColumnWithDirection = castlingTile.Position.Column - checkableTwp.Position.Column;
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
					return castlingTileByCheckableTilePos;
				}

				var checkablePos = new Position(checkableTwp.Position.Column + 2 * sign, checkableTwp.Position.Row);
				var castlingPos = new Position(checkablePos.Column - 1 * sign, checkablePos.Row);
				castlingTileByCheckableTilePos.Add(checkablePos, (castlingTile, castlingPos));
			}

			return castlingTileByCheckableTilePos;
		}

		public static IEnumerable<TileWithPiece> GetOpponentTiles(Dictionary<int, IEnumerable<TileWithPiece>> tbp, int playerId)
		{
			return tbp.Where(kvp => kvp.Key != playerId).SelectMany(kvp => kvp.Value);
		}

		public (TileWithPiece movedTileWithPiece, IEnumerable<Tile> changedTiles, Tile[,] tiles) MovePiece(TileWithPiece twp, Position pos, Dictionary<Position, (TileWithCastlingPiece, Position)> 
                castlingTileByCheckableTilePosition)
		{
			var playerId = twp.Piece.PlayerId;
			var tilesByP = tilesByPlayer[twp.Piece.PlayerId];
			var moveOutput = Board.MovePiece(twp, pos, boardTiles, tilesByP);
			(Tile beforeMoveTile, TileWithPiece afterMoveTile, Tile[,] tilesAfterMove, IEnumerable<TileWithPiece> playerTilePiecesAfterMove)? castleOutput = null;
			if (castlingTileByCheckableTilePosition.Keys.Any(p => p == pos))
			{
				var tuple = castlingTileByCheckableTilePosition[pos];
				castleOutput = Board.MovePiece(tuple.Item1, tuple.Item2, moveOutput.tilesAfterMove, moveOutput.playerTilePiecesAfterMove);
				moveOutput.tilesAfterMove = castleOutput.Value.tilesAfterMove;
				moveOutput.playerTilePiecesAfterMove = castleOutput.Value.playerTilePiecesAfterMove;
			}
			boardTiles = moveOutput.tilesAfterMove;
			tilesByPlayer[playerId] = moveOutput.playerTilePiecesAfterMove;
			var changedTiles = new[]
			{
				moveOutput.beforeMoveTile, castleOutput?.beforeMoveTile, moveOutput.afterMoveTile, castleOutput?.afterMoveTile
			}.Where(t => t != null);
			
			return (moveOutput.afterMoveTile, changedTiles, boardTiles);
		}

		public (Player player, Tile checkTile) IsPlayerInCheck(int playerId,
			PieceType checkablePieceType,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc)
		{
			var checkablePieceTile = GetCheckableTileWithPiece(playerId);
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerId);
			var checkType = Board.IsInCheck(checkablePieceTile, movesForPieceTypeFunc, opponentTiles, boardTiles, tileByStartPos, tilesByPlayer[playerId]);
			return (new Player(playerId, checkType), checkablePieceTile);
		}

		private TileWithCheckablePiece GetCheckableTileWithPiece(int playerId)
		{
			return (TileWithCheckablePiece)tilesByPlayer[playerId].First(twp => twp is TileWithCheckablePiece);
		}
	}
}