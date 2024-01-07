namespace System.Runtime.CompilerServices
{
	public static class IsExternalInit {}
}

namespace Chess
{
	public record Tile(Position Position);
	public record TileWithPiece(Position Position, Piece Piece) : Tile(Position);
}