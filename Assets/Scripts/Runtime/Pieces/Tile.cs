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

	public record TileWithPiece(Position Position, Piece Piece) : Tile(Position), IComparable<TileWithPiece>
	{
		public int CompareTo(TileWithPiece other)
		{
			
			var tileComparison = base.CompareTo(other);
			return tileComparison != 0 ? tileComparison : Piece.CompareTo(other.Piece);
		}
	}

	public record TileWithCastlingPiece(Position Position, Piece Piece, bool HasMoved = false) : TileWithPiece(Position, Piece), IComparable<TileWithCastlingPiece>
	{
		public int CompareTo(TileWithCastlingPiece other)
		{
			var tileWithPieceComparison = base.CompareTo(other);
			return tileWithPieceComparison != 0 ? tileWithPieceComparison : HasMoved.CompareTo(other.HasMoved);
		}
	}

	public record TileWithCheckablePiece(Position Position, Piece Piece, bool HasMoved = false) : TileWithCastlingPiece(Position, Piece, HasMoved);
}