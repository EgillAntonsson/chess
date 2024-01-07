using System;
using System.Collections.Generic;

namespace Chess
{
	public abstract class Variant
	{
		public abstract VariantType VariantType { get; }
		public abstract IEnumerable<IEnumerable<Tile>> TileSetupSequence { get; }
		public abstract IEnumerable<Func<Board, bool>> EndConditions { get; }
		public virtual int NumberOfPlayers => 2;
		public virtual int PlayerIdToStart => 1;

		public abstract IEnumerable<Position> ValidMovesByType(PieceType type);

		// below functions could be moved to util class
		protected static IEnumerable<Tile> CreatePieceRow(int row, int playerId, IEnumerable<PieceType> pieceTypes)
		{
			var tiles = new List<Tile>();
			var col = 0;
			foreach (var pieceType in pieceTypes)
			{
				tiles.Add(new TileWithPiece(new Position(col, row), new Piece(pieceType, playerId)));
				col++;
			}

			return tiles;
		}

		protected static IEnumerable<Tile> CreateEmptyRow(int row, int length)
		{
			var tiles = new Tile[length];
			for (var col = 0; col < length; col++)
			{
				tiles[col] = new Tile(new Position(col, row));
			}

			return tiles;
		}
	}
}