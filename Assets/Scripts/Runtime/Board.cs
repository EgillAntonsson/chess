using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(Chess.AssemblyName.TestEditMode)]

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

		public IEnumerable<Position> FindValidMoves(Tile tile, Func<PieceType, IEnumerable<Position>> validMovesByType,
			int playerIdToMove)
		{
			if (!tile.HasPiece)
			{
				return Enumerable.Empty<Position>();
			}

			var validMoves = validMovesByType(tile.Piece.Type);
			return validMoves.Select(pos => pos + tile.Position)
				.Where(pos => IsOnBoard(pos, Size))
				.Where(pos => !(GetTile(pos).HasPiece && GetTile(pos).Piece.PlayerId == playerIdToMove));
		}

		public Tile GetTile(Position position)
		{
			return tiles[position.Column, position.Row];
		}

		// TODO: could move to a separate util class
		public static bool IsOnBoard(Position position, int size)
		{
			return position.Column >= 0 && position.Column < size && position.Row >= 0 && position.Row < size;
		}
	}
}