using System;
using System.Collections.Generic;

namespace Chess
{
	public class CustomVariant
	{
		// public CustomVariant(IEnumerable<IEnumerable<Tile>> tileSetupSequence, IEnumerable<Func<Board, bool>> endConditions, int numberOfPlayers,
		// 	Func<PieceType, IEnumerable<Position>> validMovesByType)
		// {
		// 	BoardAtStart = tileSetupSequence;
		// 	EndConditions = endConditions;
		// 	NumberOfPlayers = numberOfPlayers;
		// 	validMoves = validMovesByType;
		// }
		//
		// private readonly Func<PieceType, IEnumerable<Position>> validMoves;
		//
		// public override VariantType VariantType => VariantType.Custom;
		// public override IEnumerable<IEnumerable<Tile>> BoardAtStart { get; }
		// public override IEnumerable<Func<Board, bool>> EndConditions { get; }
		// public override int NumberOfPlayers { get; }
		//
		// public override IEnumerable<Position> ValidMovesByType(PieceType type, int playerId)
		// {
		// 	return validMoves(type);
		// }
	}
}