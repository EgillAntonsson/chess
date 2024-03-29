namespace Chess
{
	public record Tile(Position Position);

	public record TileWithPiece(Position Position, Piece Piece) : Tile(Position);

	public record TileWithCastlingPiece(Position Position, Piece Piece, bool HasMoved = false) : TileWithPiece(Position, Piece);

	public record TileWithCheckablePiece(Position Position, Piece Piece, bool HasMoved = false) : TileWithCastlingPiece(Position, Piece, HasMoved);
}