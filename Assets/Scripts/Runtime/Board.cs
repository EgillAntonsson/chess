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
		public int Size { get; }
		
		public Board(IEnumerable<Tile> tilesSetup)
		{
			Size = (int)Math.Sqrt(tilesSetup.Count());
			tiles = new Tile[Size, Size];
			for (var row = 0; row < Size; row++)
			{
				for (var col = 0; col < Size; col++)
				{
					tiles[col, row] = new Tile(new Position(col, row));
				}
			}
			UpdateTiles(tilesSetup);
		}

		private Board UpdateTile(Tile tile)
		{
			tiles[tile.Position.Column, tile.Position.Row] = tile;
			return this;
		}

		private Board UpdateTiles(IEnumerable<Tile> tileSequence)
		{
			foreach (var tile in tileSequence)
			{
				UpdateTile(tile);
			}
			return this;
		}
		
		public IEnumerable<Position> FindValidMoves(Tile tile, Func<PieceType, IEnumerable<Position>> validMovesByType, int playerIdToMove)
		{
			if (!tile.HasPiece)
			{
				return Enumerable.Empty<Position>();
			}
			var validMoves = validMovesByType(tile.Piece.Type);
			return validMoves.Select(pos => pos + tile.Position).Where(pos =>
				IsOnBoard(pos, Size)).Where(pos => !tiles[pos.Column, pos.Row].HasPiece || tiles[pos.Column, pos.Row].Piece.PlayerId != playerIdToMove);
		}
		
		// TODO: could move to a separate util class
		public static bool IsOnBoard(Position position, int size)
		{
			return position.Column >= 0 && position.Column < size && position.Row >= 0 && position.Row < size;
		}
	}
}
