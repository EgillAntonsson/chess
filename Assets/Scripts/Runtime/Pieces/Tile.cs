using System;

namespace Chess
{
	public record Tile(Position Position) : IComparable<Tile>
	{
		public int CompareTo(Tile other)
		{
			return Position.CompareTo(other.Position);
		}
	}

	public record TileWithPiece(Position Position, Piece Piece, bool HasMoved = false) : Tile(Position), IComparable<TileWithPiece>
	{
		public int CompareTo(TileWithPiece other)
		{
			var tileComparison = base.CompareTo(other);
			if (tileComparison != 0)
			{
				return tileComparison;
			}

			var pieceComparison = Piece.CompareTo(other.Piece);
			return pieceComparison != 0 ? pieceComparison : HasMoved.CompareTo(other.HasMoved);
		}
	}

	public record TileWithCastlingPiece(Position Position, Piece Piece, bool HasMoved = false) : TileWithPiece(Position, Piece, HasMoved);

	public record TileWithCheckablePiece(Position Position, Piece Piece, bool HasMoved = false) : TileWithCastlingPiece(Position, Piece, HasMoved);
}