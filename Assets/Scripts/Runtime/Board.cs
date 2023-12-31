using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class Board : IEquatable<Board>
	{
		private readonly Tile[,] tiles;

		public Board(IEnumerable<Tile> variantTileSetupSequence)
		{
			var size = (int)Math.Sqrt(variantTileSetupSequence.Count());
			tiles = new Tile[size, size];
			for (var col = 0; col < size; col++)
			{
				for (var row = 0; row < size; row++)
				{
					tiles[col, row] = new Tile(new Position(col, row));
				}
			}
			UpdatePieces(variantTileSetupSequence);
		}

		public Board UpdatePiece(Tile tile)
		{
			tiles[tile.Position.Column, tile.Position.Row] = tile;
			return this;
		}

		public Board UpdatePieces(IEnumerable<Tile> tileSequence)
		{
			foreach (var tile in tileSequence)
			{
				UpdatePiece(tile);
			}
			return this;
		}

		public bool Equals(Board other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return tiles.Cast<Tile>().SequenceEqual(other.tiles.Cast<Tile>());
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Board)obj);
		}

		public override int GetHashCode()
		{
			return tiles != null ? tiles.GetHashCode() : 0;
		}

		public static bool operator ==(Board left, Board right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Board left, Board right)
		{
			return !Equals(left, right);
		}
	}
}
