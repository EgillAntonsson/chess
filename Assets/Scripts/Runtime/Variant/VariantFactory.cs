
using System;
using System.Collections.Generic;

namespace Chess
{

	public static class VariantFactory
	{
		public static Variant Create(VariantType variantType)
		{
			return variantType switch
			{
				VariantType.Standard => new StandardVariant(),
				VariantType.Custom => throw new ApplicationException(
					"VariantType.Custom should be called with the parameterized Create method overload."),
				_ => throw new NotImplementedException($"Variant '{variantType}' is not implemented.")
			};
		}
		
		// public static Variant Create(IEnumerable<Tile> tileSetupSequence, IEnumerable<Func<Board, bool>> endConditions, int numberOfPlayers)
		// {
		// 	return new CustomVariant(tileSetupSequence, endConditions, numberOfPlayers);
		// }
	}
}
