using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public class Board
	{
		public static (Tile[,] boardTiles, Dictionary<Position, PieceType> pieceTypeByStartPositions) Create(string boardTiles)
		{
			var pieceTypeByStartPositions = new Dictionary<Position, PieceType>();
			var tiles = ConvertToTile2dArray(boardTiles, pieceTypeByStartPositions);
			return (tiles, pieceTypeByStartPositions);
		}

		public static Tile[,] ConvertToTile2dArray(string tiles, Dictionary<Position, PieceType> typeByStartPositions = null)
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

		private static PieceType GetPieceTypeFromChar(char c)
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

		public static IEnumerable<Position> FindValidMoves(TileWithPiece tileWithPiece,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
			int playerIdToMove,
			Tile[,] boardTiles,
			Dictionary<Position, PieceType> pieceTypeByStartPositions)
		{
			var movesForPieceType = movesForPieceTypeFunc(tileWithPiece.Piece.Type, playerIdToMove);
			var moves = new List<Move>();
			var boardSize = GetSize(boardTiles);
			foreach (var move in movesForPieceType)
			{
				moves.AddRange(Enumerable.Range(1, move.MoveType == MoveType.Infinite ? boardSize - 1 : 1).Select(i => move with { Position = move.Position * i }));
			}
			var hasInBetweenPieceByPosition = new Dictionary<Position, bool>();

			return moves.Select(m => m with { Position = tileWithPiece.Position + m.Position })
				.Where(m => IsOnBoard(m.Position, boardSize))
				.OrderBy(m => Position.GridDistance(tileWithPiece.Position, m.Position))
				.Where(m =>
				{
					var bTile = GetTile(m.Position, boardTiles);

					var canMove = m.MoveConstraints == MoveConstraints.None
						|| m.MoveConstraints is MoveConstraints.FirstMoveOnly
						&& pieceTypeByStartPositions.ContainsKey(tileWithPiece.Position)
						&& pieceTypeByStartPositions[tileWithPiece.Position] == tileWithPiece.Piece.Type;

					var gridDistance = Position.GridDistance(tileWithPiece.Position, m.Position);

					if (m.MoveType != MoveType.Infinite || gridDistance <= 1)
					{
						return bTile is not TileWithPiece && m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Move) && canMove
							|| bTile is TileWithPiece twp && twp.Piece.PlayerId != playerIdToMove && m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Capture);
					}

					var posNormal = Position.GridNormal(m.Position, tileWithPiece.Position);
					if (hasInBetweenPieceByPosition.ContainsKey(posNormal))
					{
						return false;
					}

					var inBetweenPos = m.Position + posNormal;
					if (GetTile(inBetweenPos, boardTiles) is not TileWithPiece)
						return bTile is not TileWithPiece && m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Move) && canMove
							|| bTile is TileWithPiece twp && twp.Piece.PlayerId != playerIdToMove && m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Capture);
					hasInBetweenPieceByPosition[posNormal] = true;
					return false;
				})
				.Select(m => m.Position);
		}

		/// <summary>
		/// For now we take it as given that it is a square board this we only need to get the length of one dimension.
		/// </summary>
		/// <param name="boardTiles"></param>
		public static int GetSize(Tile[,] boardTiles)
		{
			return boardTiles.GetLength(0);
		}

		public static Tile GetTile(Position position, Tile[,] boardTiles)
		{
			return boardTiles[position.Column, position.Row];
		}

		private static bool IsOnBoard(Position position, int size)
		{
			return position.Column >= 0 && position.Column < size && position.Row >= 0 && position.Row < size;
		}

		public static (Tile beforeMoveTile, Tile afterMoveTile) MovePiece(TileWithPiece twp, Position pos, Tile[,] boardTiles)
		{
			var beforeMoveTile = boardTiles[twp.Position.Column, twp.Position.Row] = new Tile(twp.Position);
			var afterMoveTile = boardTiles[pos.Column, pos.Row] = twp with { Position = pos };
			return (beforeMoveTile, afterMoveTile);
		}

		public static (bool isCheck, bool isCheckMate, Tile checkTile) IsCheck(int playerId, Tile[,] boardTiles)
		{
			// TODO: implement
			return (false, false, null);
		}
	}
}