using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class Board
	{
		private readonly Tile[,] tiles;
		public Tile[,] Tiles => (Tile[,])tiles.Clone();
		public int Size => (int)Math.Sqrt(tiles.Length);

		public Board(IEnumerable<IEnumerable<Tile>> tilesSetup)
		{
			tiles = tilesSetup.ToRectangularArray();
		}

		public IEnumerable<Position> FindValidMoves(TileWithPiece tile,
			Func<PieceType, IEnumerable<Position>> validMovesForPieceType,
			int playerIdToMove)
		{
			return validMovesForPieceType(tile.Piece.Type)
				.Select(pos => pos + tile.Position)
				.Where(pos => IsOnBoard(pos, Size))
				.Where(pos =>
					{
						var tileFromBoard = GetTileFromBoard(pos);
						return tileFromBoard is not TileWithPiece twp || twp.Piece.PlayerId != playerIdToMove;
					}
				);
		}

		private Tile GetTileFromBoard(Position position)
		{
			return tiles[position.Column, position.Row];
		}

		private static bool IsOnBoard(Position position, int size)
		{
			return position.Column >= 0 && position.Column < size && position.Row >= 0 && position.Row < size;
		}
	}
}