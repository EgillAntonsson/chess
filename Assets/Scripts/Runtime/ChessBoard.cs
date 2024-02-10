using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
			return Board.FindValidMoves(tileWithPiece, movesForPieceTypeFunc, playerIdToMove, boardTiles, tileByStartPos, tilesByPlayer);
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
			return Board.IsInCheck(playerId, movesForPieceTypeFunc, tilesByPlayer, boardTiles, tileByStartPos);
		}
	}
}