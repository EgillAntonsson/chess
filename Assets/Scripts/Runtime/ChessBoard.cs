using System;
using System.Collections.Generic;

namespace Chess
{
	public class ChessBoard
	{
		private Tile[,] boardTiles;
		private Dictionary<Position, PieceType> pieceTypeByStartPositions;

		public Tile[,] Create(string tiles)
		{
			(boardTiles, pieceTypeByStartPositions) = Board.Create(tiles);
			return boardTiles;
		}

		public IEnumerable<Position> FindValidMoves(TileWithPiece tileWithPiece,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
			int playerIdToMove)
		{
			return Board.FindValidMoves(tileWithPiece, movesForPieceTypeFunc, playerIdToMove, boardTiles, pieceTypeByStartPositions);
		}

		public (Tile beforeMoveTile, Tile afterMoveTile) MovePiece(TileWithPiece twp, Position pos)
		{
			return Board.MovePiece(twp, pos, boardTiles);
		}
		
		public (bool isCheck, bool isCheckMate, Tile checkTile) IsCheck(int playerId)
		{
			return Board.IsCheck(playerId, boardTiles);
		}
	}
}