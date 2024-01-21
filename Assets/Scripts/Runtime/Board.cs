using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class Board
	{
		private readonly Tile[,] tiles;
		private readonly Dictionary<Position, PieceType> pieceTypeByStartPositions;
		public Tile[,] Tiles => (Tile[,])tiles.Clone();
		public int Size => (int)Math.Sqrt(tiles.Length);

		public Board(string tilePositions)
		{
			pieceTypeByStartPositions = new Dictionary<Position, PieceType>();
			tiles = ConvertToTileArray(tilePositions, pieceTypeByStartPositions);
		}

		public static Tile[,] ConvertToTileArray(string tiles, Dictionary<Position, PieceType> typeByStartPositions = null)
		{
			var rows = tiles.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
			var t = new Tile[rows.Length, rows.Length];
			for (var row = rows.Length - 1; row >= 0; row--)
			{
				var columns = rows[row].Split(' ', StringSplitOptions.RemoveEmptyEntries);
				var r = Math.Abs(row - (rows.Length - 1));
				for (var c = 0; c < columns.Length; c++)
				{
					var posString = columns[c].Trim();
					if (posString == "--")
						t[c, r] = new Tile(new Position(c, r));
					else
					{
						var pieceType = GetPieceTypeFromChar(posString[0]);
						var playerId = int.Parse(posString[1].ToString());
						t[c, r] = new TileWithPiece(new Position(c, r), new Piece(pieceType, playerId));
						typeByStartPositions?.Add(new Position(c, r), pieceType);
					}
				}
			}

			return t;
		}

		public static PieceType GetPieceTypeFromChar(char c)
		{
			c = char.ToUpper(c);
			return c switch
			{
				'P' => PieceType.Pawn,
				'N' => PieceType.Knight,
				'B' => PieceType.Bishop,
				'R' => PieceType.Rook,
				'Q' => PieceType.Queen,
				'K' => PieceType.King,
				_ => throw new ArgumentOutOfRangeException(nameof(c), c, $"Char {c} not handled.")
			};
		}

		public IEnumerable<Position> FindValidMoves(TileWithPiece tileWithPiece,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
			int playerIdToMove,
			string tilePositions = null)
		{
			var movesForPieceType = movesForPieceTypeFunc(tileWithPiece.Piece.Type, playerIdToMove);
			var moves = new List<Move>();
			foreach (var move in movesForPieceType)
			{
				moves.AddRange(Enumerable.Range(1, move.MoveType == MoveType.Infinite ? Size - 1 : 1).Select(i => move with { Position = move.Position * i }));
			}
			var hasInBetweenPieceByPosition = new Dictionary<Position, bool>();

			var sel = moves.Select(m => m with { Position = tileWithPiece.Position + m.Position });
			var w = sel.Where(m => IsOnBoard(m.Position, Size)).OrderBy(m => Position.GridDistance(tileWithPiece.Position, m.Position));
			var w2 = w.Where(m =>
			{
				var bTile = tilePositions == null ? GetTile(m.Position) : GetTile(m.Position, tilePositions);

				var canMove = m.MoveConstraints == MoveConstraints.None
					|| m.MoveConstraints is MoveConstraints.FirstMoveOnly
					&& pieceTypeByStartPositions.ContainsKey(tileWithPiece.Position)
					&& pieceTypeByStartPositions[tileWithPiece.Position] == tileWithPiece.Piece.Type;
				
				var gridDistance = Position.GridDistance(tileWithPiece.Position, m.Position);
				
				if (m.MoveType == MoveType.Infinite && gridDistance > 1)
				{
					var posNormal = Position.GridNormal(m.Position, tileWithPiece.Position);
					if (hasInBetweenPieceByPosition.ContainsKey(posNormal))
					{
						return false;
					}
					var inBetweenPos = m.Position + posNormal;
					if (tilePositions == null ? GetTile(inBetweenPos) is TileWithPiece : GetTile(inBetweenPos, tilePositions) is TileWithPiece)
					{
						hasInBetweenPieceByPosition[posNormal] = true;
						return false;
					}
				}

				return bTile is not TileWithPiece && m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Move) && canMove
					|| bTile is TileWithPiece twp && twp.Piece.PlayerId != playerIdToMove && m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Capture);
			});

			return w2.Select(m => m.Position);
		}

		public Tile GetTile(Position position)
		{
			return tiles[position.Column, position.Row];
		}

		public static Tile GetTile(Position position, string tilePositions)
		{
			return ConvertToTileArray(tilePositions)[position.Column, position.Row];
		}

		private static bool IsOnBoard(Position position, int size)
		{
			return position.Column >= 0 && position.Column < size && position.Row >= 0 && position.Row < size;
		}

		public (Tile beforeMoveTile, Tile afterMoveTile) MovePiece(TileWithPiece twp, Position pos)
		{
			var beforeMoveTile = tiles[twp.Position.Column, twp.Position.Row] = new Tile(twp.Position);
			var afterMoveTile = tiles[pos.Column, pos.Row] = twp with { Position = pos };
			return (beforeMoveTile, afterMoveTile);
		}
	}
}