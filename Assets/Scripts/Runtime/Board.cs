using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	public static class Board
	{
		public static (Tile[,] boardTiles, Dictionary<Position, TileWithPiece> tileByStartPos, Dictionary<int, IEnumerable<TileWithPiece>> tilesByPlayer)
			Create(string boardTiles, PieceType checkablePieceType, PieceType castlingPieceType)
		{
			return ConvertBoardStringToTiles(boardTiles, checkablePieceType, castlingPieceType);
		}

		private static (Tile[,] tiles, Dictionary<Position, TileWithPiece> tileByStartPos, Dictionary<int, IEnumerable<TileWithPiece>> tilesByPlayer)
			ConvertBoardStringToTiles(string tiles, PieceType checkablePieceType, PieceType castlingPieceType)
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
						var c1 = posString[0];
						c1 = char.ToUpper(c1);
						var pieceType = c1 switch
						{
							'P' => PieceType.Pawn,
							'N' => PieceType.Knight,
							'B' => PieceType.Bishop,
							'R' => PieceType.Rook,
							'Q' => PieceType.Queen,
							'K' => PieceType.King,
							_ => throw new ArgumentOutOfRangeException(nameof(c1), c1, $"Char {c1} not handled.")
						};
						var playerId = int.Parse(posString[1].ToString());
						var pos = new Position(c, r);
						var piece = new Piece(pieceType, playerId);
						TileWithPiece twp;
						if (piece.Type == checkablePieceType)
						{
							twp = new TileWithCheckablePiece(pos, piece);
						}
						else if (piece.Type == castlingPieceType)
						{
							twp = new TileWithCastlingPiece(pos, piece);
						}
						else
						{
							twp = new TileWithPiece(pos, piece);
						}

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

		public static IEnumerable<Position> FindMovePositions(TileWithPiece tileWithPiece,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
			int playerIdToMove,
			Tile[,] boardTiles,
			Dictionary<Position, TileWithPiece> tileByStartPos,
			MoveCaptureFlag moveFilter = MoveCaptureFlag.Move | MoveCaptureFlag.Capture)
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
				|| m.MoveConstraint is MoveConstraint.FirstMoveOnly && tileByStartPos.ContainsKey(tileWithPiece.Position) && tileByStartPos[tileWithPiece.Position].Piece.Type == tileWithPiece.Piece.Type
				|| m.MoveConstraint is MoveConstraint.CanMoveIfNotThreatenedCapture;
			return tileToMoveTo is not TileWithPiece && canMove;
		}

		/// <summary>
		/// For now, we take it as given that it is a square board, thus we only need to get the length of one dimension.
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

		public static (TileWithPiece promotedTile, Tile[,] tiles) PromotePiece(TileWithPiece tile, PieceType toType, Tile[,] tiles)
		{
			var promotedTile = tile with { Piece = new Piece(toType, tile.Piece.PlayerId) };
			var tilesClone = (Tile[,])tiles.Clone();
			tilesClone[promotedTile.Position.Column, promotedTile.Position.Row] = promotedTile;
			return (promotedTile, tilesClone);
		}

		public static (Tile beforeMoveTile, TileWithPiece afterMoveTile, Tile[,] tilesAfterMove, IEnumerable<TileWithPiece> playerTilePiecesAfterMove)
			MovePiece(TileWithPiece twp,
				Position pos,
				Tile[,] ts,
				IEnumerable<TileWithPiece> playerTilePieces)
		{
			var (removedTile, tilesClone) = RemovePiece(twp, ts);
			var firstMove = twp.HasMoved == false;
			var amt = twp with { Position = pos, HasMoved = true, FirstMove = firstMove};
			tilesClone[pos.Column, pos.Row] = amt;
			var tilesByPlayerAfterMove = playerTilePieces.Where(t => t != twp).Append(amt);
			return (removedTile, amt, tilesClone, tilesByPlayerAfterMove);
		}

		public static (Tile removedTile, Tile[,] tilesClone) RemovePiece(TileWithPiece twp, Tile[,] tiles)
		{
			var tilesClone = (Tile[,])tiles.Clone();
			var removedTile = tilesClone[twp.Position.Column, twp.Position.Row] = new Tile(twp.Position);
			return (removedTile, tilesClone);
		}

		public static bool IsTilePieceInCheck(TileWithPiece checkableTilePiece,
				Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc,
				IEnumerable<TileWithPiece> opponentTiles,
				Tile[,] boardTiles,
				Dictionary<Position, TileWithPiece> tileByStartPos)
		{
			var oppMovePosLists = opponentTiles.Select(twp => FindMovePositions(twp, movesForPieceTypeFunc, twp.Piece.PlayerId, boardTiles, tileByStartPos, MoveCaptureFlag.Capture));
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
			// TODO: try to remove the returning and passed in playerTilePiece into this class. The calling class should rather just cache and send the player pieces that are needed.
			var oppTiles = opponentTiles as TileWithPiece[] ?? opponentTiles.ToArray();
			var isTilePieceInCheck = IsTilePieceInCheck(checkableTilePiece, movesForPieceTypeFunc, oppTiles, boardTiles, tileByStartPos);

			if (!isTilePieceInCheck) return CheckType.NoCheck;

			const MoveCaptureFlag moveFilter = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			var movePoses = FindMovePositions(checkableTilePiece, movesForPieceTypeFunc, 1, boardTiles, tileByStartPos, moveFilter);

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
			var checkableIsMoved = moveTilePiece is TileWithCheckablePiece;
			var movedTuple = MovePiece(moveTilePiece, pos, boardTiles, playerTilePieces);
			
			return IsTilePieceInCheck(checkableIsMoved ? movedTuple.afterMoveTile : checkableTilePiece, movesForPieceTypeFunc, opponentTiles, movedTuple.tilesAfterMove, tileByStartPos);
		}
	}
}