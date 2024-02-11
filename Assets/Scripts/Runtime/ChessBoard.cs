using System;
using System.Collections.Generic;
using System.Linq;

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

		public IEnumerable<Position> FindValidMoves(TileWithPiece tileWithPiece,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
			int playerIdToMove)
		{
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerIdToMove);
			return Board.FindMoves(tileWithPiece, movesForPieceTypeFunc, playerIdToMove, boardTiles, tileByStartPos, opponentTiles);
		}

		public static IEnumerable<TileWithPiece> GetOpponentTiles(Dictionary<int, IEnumerable<TileWithPiece>> tbp, int playerId)
		{
			return tbp.Where(kvp => kvp.Key != playerId).SelectMany(kvp => kvp.Value);
		}

		public (Tile beforeMoveTile, TileWithPiece afterMoveTile) MovePiece(TileWithPiece twp, Position pos)
		{
			var (beforeMoveTile, afterMoveTile) = Board.MovePiece(twp, pos, boardTiles);
			var playerId = twp.Piece.PlayerId;
			var tiles = tilesByPlayer[playerId].Where(t => t != beforeMoveTile);
			tilesByPlayer[playerId] = tiles.Append(afterMoveTile);
			return (beforeMoveTile, afterMoveTile);
		}
		
		public (CheckType checktype, Tile checkTile) IsCheck(int playerId, Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc)
		{
			var kingTile = tilesByPlayer[playerId].First(twp => twp.Piece.Type == PieceType.King);
			
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerId);
			return (Board.IsInCheck(kingTile, movesForPieceTypeFunc, opponentTiles, boardTiles, tileByStartPos), kingTile);
		}
	}
}