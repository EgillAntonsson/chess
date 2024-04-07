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

	public record TileWithPiece(Position Position, Piece Piece, Position PieceStartPosition, bool HasMoved = false, bool FirstMove = false)
		: Tile(Position), IComparable<TileWithPiece>
	{
		// public bool HasMoved => Position - PieceStartPosition != new Position(0, 0);
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

			var pieceStartPositionComparison = PieceStartPosition.CompareTo(other.PieceStartPosition);
			if (pieceStartPositionComparison != 0)
			{
				return pieceStartPositionComparison;
			}

			var hasMovedComparison = HasMoved.CompareTo(other.HasMoved);
			if (hasMovedComparison != 0)
			{
				return hasMovedComparison;
			}

			return FirstMove.CompareTo(other.FirstMove);
		}
	}

	public record TileWithCastlingPiece(Position Position, Piece Piece, Position PieceStartPosition, bool HasMoved = false, bool FirstMove = false)
		: TileWithPiece(Position, Piece, PieceStartPosition, HasMoved, FirstMove);

	public record TileWithCheckablePiece(Position Position, Piece Piece,Position PieceStartPosition, bool HasMoved = false, bool FirstMove = false)
		: TileWithCastlingPiece(Position, Piece, PieceStartPosition, HasMoved, FirstMove);
}