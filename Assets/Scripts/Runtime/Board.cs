using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public static class Board
	{
		public static (Tile[,] boardTiles, Dictionary<Position, TileWithPiece> tileByStartPos, Dictionary<int, IEnumerable<TileWithPiece>> tilesByPlayer)
			Create(string boardTiles)
		{
			return ConvertBoardStringToTiles(boardTiles);
		}

		public static (Tile[,] tiles, Dictionary<Position, TileWithPiece> tileByStartPos, Dictionary<int, IEnumerable<TileWithPiece>> tilesByPlayer)
			ConvertBoardStringToTiles(string tiles)
		{
			var tilesByPlayer = new Dictionary<int, List<TileWithPiece>>();
			var tileByStartPos = new Dictionary<Position, TileWithPiece>();
			var rows = tiles.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
			var theTiles = new Tile[rows.Length, rows.Length];

			for (var row = rows.Length - 1; row >= 0; row--)
			{
				var columns = rows[row].Split(' ', StringSplitOptions.RemoveEmptyEntries);
				var r = Math.Abs(row - (rows.Length - 1));
				for (var c = 0; c < columns.Length; c++)
				{
					var posString = columns[c].Trim();
					if (posString == "--")
						theTiles[c, r] = new Tile(new Position(c, r));
					else
					{
						var pieceType = GetPieceTypeFromChar(posString[0]);
						var playerId = int.Parse(posString[1].ToString());
						var twp = new TileWithPiece(new Position(c, r), new Piece(pieceType, playerId));
						theTiles[c, r] = twp;

						tilesByPlayer.TryGetValue(playerId, out var tileByType);
						tilesByPlayer[playerId] = tileByType ?? new List<TileWithPiece>();
						tilesByPlayer[playerId].Add(twp);

						tileByStartPos.Add(new Position(c, r), twp);
					}
				}
			}

			var tilesByPlayerRet = tilesByPlayer.ToDictionary(kvp => kvp.Key, kvp => (IEnumerable<TileWithPiece>)kvp.Value);

			return (theTiles, tileByStartPos, tilesByPlayerRet);
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

		public static IEnumerable<Position> FindMoves(TileWithPiece tileWithPiece,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
			int playerIdToMove,
			Tile[,] boardTiles,
			Dictionary<Position, TileWithPiece> tileByStartPos,
			IEnumerable<TileWithPiece> opponentTiles,
			bool recursive = true)
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

					var gridDistance = Position.GridDistance(tileWithPiece.Position, m.Position);

					if (m.MoveType != MoveType.Infinite || gridDistance <= 1)
					{
						return CanMoveOrCapture();
					}

					var posNormal = Position.GridNormal(m.Position, tileWithPiece.Position);
					if (hasInBetweenPieceByPosition.ContainsKey(posNormal))
					{
						return false;
					}

					var inBetweenPos = m.Position + posNormal;
					if (GetTile(inBetweenPos, boardTiles) is not TileWithPiece)
					{
						return CanMoveOrCapture();
					}

					hasInBetweenPieceByPosition[posNormal] = true;
					return false;

					bool CanMoveOrCapture() =>
						bTile is TileWithPiece twp && twp.Piece.PlayerId != playerIdToMove && m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Capture)
						|| bTile is not TileWithPiece && CanMove();

					bool CanMove()
					{
						var canMove = m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Move);
						canMove = canMove && m.MoveConstraint is MoveConstraint.None
							|| m.MoveConstraint is MoveConstraint.FirstMoveOnly
							&& tileByStartPos.ContainsKey(tileWithPiece.Position)
							&& tileByStartPos[tileWithPiece.Position].Piece.Type == tileWithPiece.Piece.Type
							|| m.MoveConstraint is MoveConstraint.CanMoveIfNotThreatenedCapture
							&& recursive
							&& IsInCheck(new TileWithPiece(bTile.Position, tileWithPiece.Piece), movesForPieceTypeFunc, opponentTiles, boardTiles, tileByStartPos) == CheckType.NoCheck;
						return canMove;
					}
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

		public static (Tile beforeMoveTile, TileWithPiece afterMoveTile) MovePiece(TileWithPiece twp, Position pos, Tile[,] boardTiles)
		{
			var beforeMoveTile = boardTiles[twp.Position.Column, twp.Position.Row] = new Tile(twp.Position);
			var afterMoveTile = (TileWithPiece)(boardTiles[pos.Column, pos.Row] = twp with { Position = pos });
			return (beforeMoveTile, afterMoveTile);
		}

		public static CheckType IsInCheck(TileWithPiece checkableTilePiece,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
			IEnumerable<TileWithPiece> opponentTiles,
			Tile[,] boardTiles,
			Dictionary<Position, TileWithPiece> tileByStartPos)
		{
			;
			var oppTiles = opponentTiles as TileWithPiece[] ?? opponentTiles.ToArray();
			var c = oppTiles.SelectMany(twp => FindMoves(twp, movesForPieceTypeFunc, twp.Piece.PlayerId, boardTiles, tileByStartPos, oppTiles));
			var d = c.Where(pos => pos == checkableTilePiece.Position);

			if (!d.Any()) return CheckType.NoCheck;

			var moves = FindMoves(checkableTilePiece, movesForPieceTypeFunc, 1, boardTiles, tileByStartPos, oppTiles, false);
			if (moves.Any())
			{
				return CheckType.Check;
			}

			return CheckType.CheckMate;
		}
	}
}