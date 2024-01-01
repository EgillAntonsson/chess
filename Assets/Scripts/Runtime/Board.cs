
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(Chess.AssemblyName.TestEditMode)]

namespace Chess
{
	public class Board : IEquatable<Board>
	{
		private readonly Tile[,] tiles;
		public Tile[,] Tiles => (Tile[,])tiles.Clone();
		public int Size { get; }
		
		public Board(IEnumerable<Tile> variantTileSetupSequence)
		{
			Size = (int)Math.Sqrt(variantTileSetupSequence.Count());
			tiles = new Tile[Size, Size];
			for (var row = 0; row < Size; row++)
			{
				for (var col = 0; col < Size; col++)
				{
					tiles[col, row] = new Tile(new Position(col, row));
				}
			}
			UpdateTiles(variantTileSetupSequence);
		}

		// public Tile[,] GetTiles()
		// {
		// 	return tiles;
		// }
		

		internal Board UpdateTile(Tile tile)
		{
			tiles[tile.Position.Column, tile.Position.Row] = tile;
			return this;
		}

		internal Board UpdateTiles(IEnumerable<Tile> tileSequence)
		{
			foreach (var tile in tileSequence)
			{
				UpdateTile(tile);
			}
			return this;
		}

		public bool Equals(Board other)
		{
			if (ReferenceEquals(null, other)) return false;
			return ReferenceEquals(this, other) || tiles.Cast<Tile>().SequenceEqual(other.tiles.Cast<Tile>());
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
