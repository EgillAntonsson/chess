namespace System.Runtime.CompilerServices
{
	/// <summary>
	/// Unity needs this for full record support, asi it uses init only setters.
	/// https://docs.unity3d.com/2022.3/Documentation/Manual/CSharpCompiler.html
	/// </summary>
	public static class IsExternalInit {}
}

namespace Chess
{
	public record Tile(Position Position);
	
	public record TileWithPiece(Position Position, Piece Piece) : Tile(Position);
}