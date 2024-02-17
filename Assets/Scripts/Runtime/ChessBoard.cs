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
				bool isCheckablePiece,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
			int playerIdToMove)
		{
			const MoveCaptureFlag moveFilter = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			var poses = Board.FindMovesPos(tileWithPiece, movesForPieceTypeFunc, playerIdToMove, boardTiles, tileByStartPos, moveFilter);
			if (!isCheckablePiece)
			{
				return poses;
			}
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerIdToMove);
			return poses.Where(pos =>
			{
				var ret = Board.MovePiece(tileWithPiece, pos, boardTiles, tilesByPlayer[playerIdToMove]);
				return !Board.IsTilePieceInCheck(ret.afterMoveTile, movesForPieceTypeFunc, opponentTiles, ret.tilesAfterMove, tileByStartPos);
			});
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
		
		public (CheckType checktype, Tile checkTile) IsPlayerInCheck(int playerId,
				PieceType checkablePieceType,
				Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc)
		{
			var checkablePieceTile = tilesByPlayer[playerId].First(twp => twp.Piece.Type == checkablePieceType);
			var opponentTiles	 = GetOpponentTiles(tilesByPlayer, playerId);
			var checkType = Board.IsInCheck(checkablePieceTile, movesForPieceTypeFunc, opponentTiles, boardTiles, tileByStartPos, tilesByPlayer[playerId]);
			return (checkType, checkablePieceTile);
		}
	}
}