using System;

namespace Chess
{
	public readonly record struct Piece(PieceType Type, int PlayerId) : IComparable<Piece>
	{
		public int CompareTo(Piece other)
		{
			var typeComparison = Type.CompareTo(other.Type);
			return typeComparison != 0 ? typeComparison : PlayerId.CompareTo(other.PlayerId);
		}
	}
}
