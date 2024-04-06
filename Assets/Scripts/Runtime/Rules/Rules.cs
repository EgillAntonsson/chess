using System;
using System.Collections.Generic;

namespace Chess
{
	public class Rules
	{
		public virtual string BoardAtStart => @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 P2 P2 P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
P1 P1 P1 P1 P1 P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
		
		public virtual HashSet<EndConditionType> EndConditions => new() { EndConditionType.CheckMate };

		public virtual int NumberOfPlayers => 2;
		public virtual int PlayerIdToStart => 1;
		public virtual PieceType CheckablePieceType => PieceType.King;
		public virtual PieceType CastlingPieceType => PieceType.Rook;
		/// <summary>
		/// Can not be the same as Checkable Piece Type. All capture must must have the same row position
		/// </summary>
		public virtual PieceType InPassingPieceType => PieceType.Pawn;
		public IEnumerable<Move> MovesByType(PieceType type, int playerId)
		{
			return type switch
			{
				PieceType.Knight => KnightMoves(),
				PieceType.Pawn => PawnMoves(playerId),
				PieceType.Bishop => BishopMoves(),
				PieceType.Rook => RookMoves(),
				PieceType.Queen => QueenMoves(),
				PieceType.King => KingMoves(),
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, "Must implement moves for new PieceType")
			};
		}
		protected virtual IEnumerable<Move> KnightMoves()
		{
			const MoveType moveType = MoveType.Basic;
			const MoveCaptureFlag moveAndCaptureFlagSet = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			return new Move[]
			{
				new(new Position(1, 2), moveType, moveAndCaptureFlagSet),
				new(new Position(2, 1), moveType, moveAndCaptureFlagSet),
				new(new Position(2, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(1, -2), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, -2), moveType, moveAndCaptureFlagSet),
				new(new Position(-2, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(-2, 1), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, 2), moveType, moveAndCaptureFlagSet)
			};
		}

		protected virtual IEnumerable<Move> PawnMoves(int playerId)
		{
			var row = playerId == 1 ? 1 : -1;
			const MoveType moveType = MoveType.Basic;
			return new Move[]
			{
				new(new Position(0, row), moveType, MoveCaptureFlag.Move),
				new(new Position(0, row * 2), moveType, MoveCaptureFlag.Move, MoveConstraint.FirstMoveOnly),
				new(new Position(1, row), moveType, MoveCaptureFlag.Capture),
				new(new Position(-1, row), moveType, MoveCaptureFlag.Capture)
			};
		}

		protected virtual IEnumerable<Move> QueenMoves()
		{
			const MoveType moveType = MoveType.Infinite;
			const MoveCaptureFlag moveAndCaptureFlagSet = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			return new Move[]
			{
				new(new Position(0, 1), moveType, moveAndCaptureFlagSet),
				new(new Position(1, 1), moveType, moveAndCaptureFlagSet),
				new(new Position(1, 0), moveType, moveAndCaptureFlagSet),
				new(new Position(1, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(0, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, 0), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, 1), moveType, moveAndCaptureFlagSet)
			};
		}
		
		protected virtual IEnumerable<Move> KingMoves()
		{
			const MoveType moveType = MoveType.Basic;
			const MoveConstraint moveConstraint = MoveConstraint.CanMoveIfNotThreatenedCapture;
			const MoveCaptureFlag moveAndCaptureFlagSet = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			return new Move[]
			{
				new(new Position(0, 1), moveType, moveAndCaptureFlagSet, moveConstraint),
				new(new Position(1, 1), moveType, moveAndCaptureFlagSet, moveConstraint),
				new(new Position(1, 0), moveType, moveAndCaptureFlagSet, moveConstraint),
				new(new Position(1, -1), moveType, moveAndCaptureFlagSet, moveConstraint),
				new(new Position(0, -1), moveType, moveAndCaptureFlagSet, moveConstraint),
				new(new Position(-1, -1), moveType, moveAndCaptureFlagSet, moveConstraint),
				new(new Position(-1, 0), moveType, moveAndCaptureFlagSet, moveConstraint),
				new(new Position(-1, 1), moveType, moveAndCaptureFlagSet, moveConstraint)
			};
		}
		
		protected virtual IEnumerable<Move> BishopMoves()
		{
			const MoveType moveType = MoveType.Infinite;
			const MoveCaptureFlag moveAndCaptureFlagSet = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			return new Move[]
			{
				new(new Position(1, 1), moveType, moveAndCaptureFlagSet),
				new(new Position(1, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, 1), moveType, moveAndCaptureFlagSet)
			};
		}
		
		protected virtual IEnumerable<Move> RookMoves()
		{
			const MoveType moveType = MoveType.Infinite;
			const MoveCaptureFlag moveAndCaptureFlagSet = MoveCaptureFlag.Move | MoveCaptureFlag.Capture;
			return new Move[]
			{
				new(new Position(0, 1), moveType, moveAndCaptureFlagSet),
				new(new Position(1, 0), moveType, moveAndCaptureFlagSet),
				new(new Position(0, -1), moveType, moveAndCaptureFlagSet),
				new(new Position(-1, 0), moveType, moveAndCaptureFlagSet),
			};
		}
	}
}