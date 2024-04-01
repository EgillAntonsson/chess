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

	public record TileWithPiece(Position Position, Piece Piece, bool HasMoved = false, bool FirstMove = false) : Tile(Position), IComparable<TileWithPiece>
	{
		public int CompareTo(TileWithPiece other)
		{
			var tileComparison = base.CompareTo(other);
			if (tileComparison != 0)
			{
				return tileComparison;
			}

			var pieceComparison = Piece.CompareTo(other.Piece);
			if (pieceComparison != 0)
			{
				return pieceComparison;
			}

			var hasMovedComparison = HasMoved.CompareTo(other.HasMoved);
			return hasMovedComparison != 0 ? hasMovedComparison : FirstMove.CompareTo(other.FirstMove);
		}
	}

	public record TileWithCastlingPiece(Position Position, Piece Piece, bool HasMoved = false, bool FirstMove = false) : TileWithPiece(Position, Piece, HasMoved, FirstMove);

	public record TileWithCheckablePiece(Position Position, Piece Piece, bool HasMoved = false, bool FirstMove = false) : TileWithCastlingPiece(Position, Piece, HasMoved, FirstMove);
}