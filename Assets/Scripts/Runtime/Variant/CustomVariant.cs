using System;
using System.Collections.Generic;

namespace Chess
{
	public class CustomVariant : Variant
	{
		public CustomVariant(IEnumerable<Tile> tileSetupSequence, IEnumerable<Func<ChessBoard, bool>> endConditions, int numberOfPlayers)
		{
			TileSetupSequence = tileSetupSequence;
			EndConditions = endConditions;
			NumberOfPlayers = numberOfPlayers;
		}
		
		public override VariantType VariantType => VariantType.Custom;
		public override IEnumerable<Tile> TileSetupSequence { get; }
		public override IEnumerable<Func<ChessBoard, bool>> EndConditions { get; }
		public override int NumberOfPlayers { get; }
	}

}