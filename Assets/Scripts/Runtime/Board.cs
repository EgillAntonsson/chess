using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class Board : IEquatable<Board>
	{
		private readonly Piece[,] squares;

		public Board(int size = 8)
		{
			squares = new Piece[size, size];
			for (var col = 0; col < size; col++)
			{
				for (var row = 0; row < size; row++)
				{
					squares[col, row] = new NonePiece();
				}
			}
		}

		public Board UpdatePiece(Piece piece, Position position)
		{
			squares[position.Column, position.Row] = piece;
			return this;
		}

		public Board UpdatePieces(IEnumerable<(Piece, Position)> pieces)
		{
			foreach (var (piece, position) in pieces)
			{
				UpdatePiece(piece, position);
			}
			return this;
		}

		public bool Equals(Board other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return squares.Cast<Piece>().SequenceEqual(other.squares.Cast<Piece>());
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Board)obj);
		}

		public override int GetHashCode()
		{
			return squares != null ? squares.GetHashCode() : 0;
		}

		public static bool operator ==(Board left, Board right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Board left, Board right)
		{
			return !Equals(left, right);
		}
	}
}
