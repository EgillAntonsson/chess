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

		public static IEnumerable<Position> FindMovesPos(TileWithPiece tileWithPiece,
				Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
				int playerIdToMove,
				Tile[,] boardTiles,
				Dictionary<Position, TileWithPiece> tileByStartPos,
				MoveCaptureFlag moveFilter)
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
						return ProcessMovePosFilters();
					}

					var posNormal = Position.GridNormal(m.Position, tileWithPiece.Position);
					if (hasInBetweenPieceByPosition.ContainsKey(posNormal))
					{
						return false;
					}

					var inBetweenPos = m.Position + posNormal;
					if (GetTile(inBetweenPos, boardTiles) is not TileWithPiece)
					{
						return ProcessMovePosFilters();
					}

					hasInBetweenPieceByPosition[posNormal] = true;
					return false;

					bool ProcessMovePosFilters()
					{
						return moveFilter switch
						{
								MoveCaptureFlag.Move => MovePredicate(tileWithPiece, bTile, m, tileByStartPos),
								MoveCaptureFlag.Capture => CapturePredicate(bTile, playerIdToMove, m),
								MoveCaptureFlag.Move | MoveCaptureFlag.Capture => MovePredicate(tileWithPiece, bTile, m, tileByStartPos) || CapturePredicate(bTile, playerIdToMove, m),
								_ => throw new ArgumentOutOfRangeException(nameof(moveFilter), moveFilter, null)
						};
					}
					
				})
				.Select(m => m.Position);
		}

		public static bool CapturePredicate(Tile tileToCapture, int playerIdToMove, Move m)
		{
			return tileToCapture is TileWithPiece twp && twp.Piece.PlayerId != playerIdToMove && m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Capture);
		}
		
		public static bool MovePredicate(TileWithPiece tileWithPiece, Tile tileToMoveTo, Move m, Dictionary<Position, TileWithPiece> tileByStartPos)
		{
			var canMove = m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Move);
			canMove = canMove && m.MoveConstraint is MoveConstraint.None
					|| m.MoveConstraint is MoveConstraint.FirstMoveOnly
					&& tileByStartPos.ContainsKey(tileWithPiece.Position)
					&& tileByStartPos[tileWithPiece.Position].Piece.Type == tileWithPiece.Piece.Type
					|| m.MoveConstraint is MoveConstraint.CanMoveIfNotThreatenedCapture;
			return tileToMoveTo is not TileWithPiece && canMove;
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

		public static (Tile beforeMoveTile, TileWithPiece afterMoveTile, Tile[,] tilesAfterMove, IEnumerable<TileWithPiece> playerTilePiecesAfterMove)
				MovePiece(TileWithPiece twp,
						Position pos,
						Tile[,] boardTiles,
						IEnumerable<TileWithPiece> playerTilePieces)
		{
			var boardTilesAfterMove = (Tile[,])boardTiles.Clone();
			var bmt = boardTilesAfterMove[twp.Position.Column, twp.Position.Row] = new Tile(twp.Position);
			var amt = (TileWithPiece)(boardTilesAfterMove[pos.Column, pos.Row] = twp with { Position = pos });
			
			var tilesByPlayerAfterMove = playerTilePieces.Where(t => t != twp).Append(amt);
			
			return (bmt, amt, boardTilesAfterMove, tilesByPlayerAfterMove);
		}
		
		public static bool IsTilePieceInCheck(TileWithPiece checkableTilePiece,
				Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
				IEnumerable<TileWithPiece> opponentTiles,
				Tile[,] boardTiles,
				Dictionary<Position, TileWithPiece> tileByStartPos)
		{
			var oppMovePosLists = opponentTiles.Select(twp => FindMovesPos(twp, movesForPieceTypeFunc, twp.Piece.PlayerId, boardTiles, tileByStartPos, MoveCaptureFlag.Capture));
			var oppMovePosList = oppMovePosLists.SelectMany(posList => posList);
			return oppMovePosList.Any(pos => pos == checkableTilePiece.Position);
		}

		public static CheckType IsInCheck(TileWithPiece checkableTilePiece,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
			IEnumerable<TileWithPiece> opponentTiles,
			Tile[,] boardTiles,
			Dictionary<Position, TileWithPiece> tileByStartPos,
			IEnumerable<TileWithPiece> playerTilePieces)
		{
			var oppTiles = opponentTiles as TileWithPiece[] ?? opponentTiles.ToArray();
			var isTilePieceInCheck = IsTilePieceInCheck(checkableTilePiece, movesForPieceTypeFunc, oppTiles, boardTiles, tileByStartPos);

			if (!isTilePieceInCheck) return CheckType.NoCheck;

			const MoveCaptureFlag moveFilter = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			var movePoses = FindMovesPos(checkableTilePiece, movesForPieceTypeFunc, 1, boardTiles, tileByStartPos, moveFilter);

			var posesNotInCheck = movePoses.Where(pos => !IsInCheckAfterMove(checkableTilePiece, checkableTilePiece, pos, boardTiles, playerTilePieces, tileByStartPos, oppTiles, movesForPieceTypeFunc));
			return posesNotInCheck.Any() ? CheckType.Check : CheckType.CheckMate;
		}

		public static bool IsInCheckAfterMove(TileWithPiece checkableTilePiece,
				TileWithPiece moveTilePiece,
				Position pos,
				Tile[,] boardTiles,
				IEnumerable<TileWithPiece> playerTilePieces,
				Dictionary<Position, TileWithPiece> tileByStartPos,
				IEnumerable<TileWithPiece> opponentTiles,
				Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc)
		{
			var checkableIsMoved = checkableTilePiece == moveTilePiece;
			var movedTuple = MovePiece(moveTilePiece, pos, boardTiles, playerTilePieces);
			
			return IsTilePieceInCheck(checkableIsMoved ? movedTuple.afterMoveTile : checkableTilePiece, movesForPieceTypeFunc, opponentTiles, movedTuple.tilesAfterMove, tileByStartPos);
		}
	}
}