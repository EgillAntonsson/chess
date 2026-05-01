using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Chess.Test.EditMode")]

namespace Chess
{
	public class ChessBoard
	{
		private readonly Rules rules;
		private Tile[,] boardTiles;
		private Dictionary<Position, TileWithPiece> tileByStartPos;
		private Dictionary<int, IEnumerable<TileWithPiece>> tilesByPlayer;

		public ChessBoard(Rules rules)
		{
			this.rules = rules;
		}

		public Tile[,] Create(string tiles)
		{
			(boardTiles, tileByStartPos, tilesByPlayer) = Board.Create(tiles, rules.CheckablePieceType, rules.CastlingPieceType);
			return boardTiles;
		}

		internal (Tile[,] tiles, Dictionary<int, IEnumerable<TileWithPiece>> tilesByPlayer) SetBoardState(string tiles)
		{
			(boardTiles, _, tilesByPlayer) = Board.Create(tiles, rules.CheckablePieceType, rules.CastlingPieceType);
			return (boardTiles, tilesByPlayer);
		}

		public TileWithPiece PromotePiece(TileWithPiece tile, PieceType toType)
		{
			var ret = Board.PromotePiece(tile, toType, boardTiles);
			boardTiles = ret.tiles;
			return ret.promotedTile;
		}

		public (IEnumerable<Position> movePositions, Dictionary<Position, CastlingMove> castlingMoves, IEnumerable<InPassingMove> inPassingMoves)
			FindMoves(TileWithPiece tileWithPiece, CheckType playerCheckType, IEnumerable<TileWithPiece> lastMoveOfOpponents)
		{
			var playerId = tileWithPiece.Piece.PlayerId;
			var playerTilePs = tilesByPlayer[playerId];
			var oppTilePs = GetOpponentTiles(tilesByPlayer, playerId);

			var movePositions = FilterAwayCheckedMovePositions(
				Board.FindMovePositions(tileWithPiece, rules.MoveDefinitionByType, playerId, boardTiles, tileByStartPos)
			);

			var inPassingMoves = FindInPassingMove(tileWithPiece, lastMoveOfOpponents);
			var inPassingMovePositions = inPassingMoves.Select(ic => ic.CapturePos);

			var castlingMoves = FindCastlingMoves(tileWithPiece, playerCheckType == CheckType.Check, playerTilePs, oppTilePs);

			return (movePositions.Concat(castlingMoves.Keys).Concat(inPassingMovePositions), castlingMoves, inPassingMoves);

			IEnumerable<Position> FilterAwayCheckedMovePositions(IEnumerable<Position> positions)
			{
				return positions.Where(pos => !Board.IsInCheckAfterMove(GetCheckableTileWithPiece(playerId), tileWithPiece, pos, boardTiles, playerTilePs, tileByStartPos, oppTilePs, rules.MoveDefinitionByType));
			}
		}

		private IEnumerable<InPassingMove> FindInPassingMove(TileWithPiece tileWithPiece, IEnumerable<TileWithPiece> lastMoveOfOpponents)
		{
			var inPassingPieceType = rules.InPassingPieceType;
			if (tileWithPiece.Piece.Type != inPassingPieceType)
			{
				return Enumerable.Empty<InPassingMove>();
			}

			var captureMoves = rules.MoveDefinitionByType(inPassingPieceType, tileWithPiece.Piece.PlayerId)
				.Where(m => m.MoveCaptureFlag.HasFlag(MoveCaptureFlag.Capture));
			var capturedMove = captureMoves.First();

			return lastMoveOfOpponents
				.Where(twp => twp.FirstMove && twp.Piece.Type == inPassingPieceType)
				.Where(twp => Position.GridDistance(twp.Position, tileWithPiece.Position) == 1 && twp.Position.Row == tileWithPiece.Position.Row)
				.Select(twp => new InPassingMove(new Position(twp.Position.Column, twp.Position.Row + capturedMove.Position.Row), twp));
		}

		private Dictionary<Position, CastlingMove> FindCastlingMoves(TileWithPiece tileWithPiece, bool isInCheck,
			IEnumerable<TileWithPiece> playerTilePieces,
			IEnumerable<TileWithPiece> opponentTiles)
		{
			var castlingMoves = new Dictionary<Position, CastlingMove>();
			if (tileWithPiece is not TileWithCheckablePiece checkableTwp)
			{
				return castlingMoves;
			}
			if (isInCheck || checkableTwp.HasMoved)
			{
				return castlingMoves;
			}
			var castlingTilesExcludingCheckable = playerTilePieces
				.Select(c => c as TileWithCastlingPiece)
				.Where(c => c != null)
				.Where(c => c.GetType() == typeof(TileWithCastlingPiece))
				.Where(t => t.HasMoved == false);
			foreach (var castlingTile in castlingTilesExcludingCheckable)
			{
				var nrOfColumnWithDirection = castlingTile.Position.Column - checkableTwp.Position.Column;
				var sign = Math.Sign(nrOfColumnWithDirection);
				var nrOfColumn = Math.Abs(nrOfColumnWithDirection);
				var isEmpty = true;
				for (var i = 1; i < nrOfColumn; i++)
				{
					isEmpty = Board.GetTile(new Position(checkableTwp.Position.Column + i * sign, checkableTwp.Position.Row), boardTiles) is not TileWithPiece;
					if (!isEmpty) break;
				}

				if (!isEmpty) continue;

				var columnsMoves = new[] { checkableTwp.Position.Column + 1 * sign, checkableTwp.Position.Column + 2 * sign };
				if (columnsMoves.Select(colToCheck => Board.IsInCheckAfterMove(checkableTwp,
						checkableTwp,
						new Position(colToCheck, checkableTwp.Position.Row),
						boardTiles,
						playerTilePieces,
						tileByStartPos,
						opponentTiles,
						rules.MoveDefinitionByType))
					.Any(isInCheckAfterMove => isInCheckAfterMove))
				{
					return castlingMoves;
				}

				var checkablePos = new Position(checkableTwp.Position.Column + 2 * sign, checkableTwp.Position.Row);
				var castlingPos = new Position(checkablePos.Column - 1 * sign, checkablePos.Row);
				castlingMoves.Add(checkablePos, new CastlingMove(castlingTile, castlingPos));
			}

			return castlingMoves;
		}

		public static IEnumerable<TileWithPiece> GetOpponentTiles(Dictionary<int, IEnumerable<TileWithPiece>> tbp, int playerId)
		{
			return tbp.Where(kvp => kvp.Key != playerId).SelectMany(kvp => kvp.Value);
		}

		public (TileWithPiece movedTileWithPiece, IEnumerable<Tile> changedTiles, Tile[,] tiles)
			MovePiece(TileWithPiece twp, Position pos, Dictionary<Position, CastlingMove> castlingMoves, IEnumerable<InPassingMove> inPassingMoves)
		{
			var tilesByP = tilesByPlayer[twp.Piece.PlayerId];
			var moveOutput = Board.MovePiece(twp, pos, boardTiles, tilesByP);
			(Tile beforeMoveTile, TileWithPiece afterMoveTile, Tile[,] tilesAfterMove, IEnumerable<TileWithPiece> playerTilePiecesAfterMove)? castleOutput = null;
			(Tile removedTile, Tile[,] tiles)? passingOutput = null;

			if (castlingMoves.Keys.Any(p => p == pos))
			{
				var target = castlingMoves[pos];
				castleOutput = Board.MovePiece(target.Tile, target.Destination, moveOutput.tilesAfterMove, moveOutput.playerTilePiecesAfterMove);
				moveOutput.tilesAfterMove = castleOutput.Value.tilesAfterMove;
				moveOutput.playerTilePiecesAfterMove = castleOutput.Value.playerTilePiecesAfterMove;
			}

			if (inPassingMoves.Any(ic => ic.CapturePos == pos))
			{
				var passedPieceToCapture = inPassingMoves.First(ic => ic.CapturePos == pos).PassedPiece;
				passingOutput = Board.RemovePiece(passedPieceToCapture, moveOutput.tilesAfterMove);
				moveOutput.tilesAfterMove = passingOutput.Value.tiles;
			}

			boardTiles = moveOutput.tilesAfterMove;
			tilesByPlayer = Board.GetPiecesByPlayer(boardTiles);
			var changedTiles = new[]
			{
				moveOutput.beforeMoveTile, moveOutput.afterMoveTile, castleOutput?.beforeMoveTile, castleOutput?.afterMoveTile, passingOutput?.removedTile
			}.Where(t => t != null);

			return (moveOutput.afterMoveTile, changedTiles, boardTiles);
		}

		public (Player player, Position checkTilePos) IsPlayerInCheck(int playerId,
			Func<PieceType, int, IEnumerable<Move>> movesForPieceTypeFunc)
		{
			var checkablePieceTile = GetCheckableTileWithPiece(playerId);
			var opponentTiles = GetOpponentTiles(tilesByPlayer, playerId);
			var checkType = Board.IsInCheck(checkablePieceTile, movesForPieceTypeFunc, opponentTiles, boardTiles, tileByStartPos, tilesByPlayer[playerId]);
			return (new Player(playerId, checkType), checkablePieceTile.Position);
		}

		public TileWithCheckablePiece GetCheckableTileWithPiece(int playerId)
		{
			return (TileWithCheckablePiece)tilesByPlayer[playerId].First(twp => twp is TileWithCheckablePiece);
		}
	}
}
