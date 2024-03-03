using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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

		public IEnumerable<Position> FindMoves(TileWithPiece tileWithPiece,
				bool isInCheck,
				PieceType checkablePieceType,
				Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
				int playerIdToMove)
		{
			var checkableTwp = GetCheckableTileWithPiece(playerIdToMove, checkablePieceType);
			var isCheckableTwp = tileWithPiece == checkableTwp;
			const MoveCaptureFlag moveCaptureFlags = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			var playerTilePieces = tilesByPlayer[playerIdToMove];
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerIdToMove);

			var poses = Board.FindMovePositions(tileWithPiece, movesForPieceTypeFunc, playerIdToMove, boardTiles, tileByStartPos, moveCaptureFlags);

			return !isCheckableTwp && !isInCheck ? poses : IsInCheckAfterMoveSimulation(poses);

			IEnumerable<Position> IsInCheckAfterMoveSimulation(IEnumerable<Position> positions)
			{
				return positions.Where(pos => !Board.IsInCheckAfterMove(checkableTwp, tileWithPiece, pos, boardTiles, playerTilePieces, tileByStartPos, opponentTiles, movesForPieceTypeFunc));
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