using System;

namespace Chess
{
	public struct Tile
	{
		public Position Position { get; }
		public Piece Piece { get; }
		public TileType TileType { get; }
		
		public Tile(Position position, Piece piece)
		{
			Position = position;
			Piece = piece;
			TileType = TileType.Normal;
		}

		public Tile(Position position)
		{
			Position = position;
			Piece = new Piece(PieceType.None, -1);
			TileType = TileType.Normal;
		}
		
		public Tile(Position position, Piece piece, TileType type)
		{
			Position = position;
			Piece = piece;
			TileType = type;
		}

		public bool HasPiece()
		{
			return Piece.Type != PieceType.None;
		}
		
		public bool IsEmpty()
		{
			return Piece.Type == PieceType.None;
		}
		
		public bool IsValid()
		{
			return TileType != TileType.None;
		}
	}
}