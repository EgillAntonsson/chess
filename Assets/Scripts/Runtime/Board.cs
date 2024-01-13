using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess
{
	public class Board
	{
		private readonly Tile[,] tiles;
		public Tile[,] Tiles => (Tile[,])tiles.Clone();
		public int Size => (int)Math.Sqrt(tiles.Length);

		public Board(IEnumerable<IEnumerable<Tile>> tilesSetup)
		{
			tiles = tilesSetup.ToRectangularArray();
		}

		public IEnumerable<Position> FindValidMoves(TileWithPiece tile,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceType,
			int playerIdToMove)
		{
			return movesForPieceType(tile.Piece.Type, playerIdToMove)
				.Select(m => m with { Position = m.Position + tile.Position })
				.Where(m => IsOnBoard(m.Position, Size)).Where(m =>
				{
					var bTile = GetTileFromBoard(m.Position);
					return bTile is not TileWithPiece && m.MoveType is MoveType.CaptureOrMove or MoveType.MoveOnly or MoveType.FirstMoveAddition
					       || bTile is TileWithPiece twp && twp.Piece.PlayerId != playerIdToMove && m.MoveType is MoveType.CaptureOrMove or MoveType.CaptureOnly;
				})
				.Select(m => m.Position);

		}

		private Tile GetTileFromBoard(Position position)
		{
			return tiles[position.Column, position.Row];
		}

		private static bool IsOnBoard(Position position, int size)
		{
			return position.Column >= 0 && position.Column < size && position.Row >= 0 && position.Row < size;
		}

		public TileWithPiece MovePiece(TileWithPiece tile, Position position)
		{
			// TODO: implement
			return null;
		}
	}
}