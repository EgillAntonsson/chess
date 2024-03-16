
using System;
using System.Collections.Generic;

namespace Chess
{

	public static class VariantFactory
	{
		public static Rules Create(VariantType variantType)
		{
			return variantType switch
			{
				VariantType.Standard => new StandardRules(),
				VariantType.Custom => throw new ApplicationException(
					"VariantType.Custom should be called with the parameterized Create method overload."),
				_ => throw new NotImplementedException($"Rules '{variantType}' is not implemented.")
			};
		}
		
		// public static Rules Create(IEnumerable<Tile> tileSetupSequence, IEnumerable<Func<Board, bool>> endConditions, int numberOfPlayers)
		// {
		// 	return new CustomVariant(tileSetupSequence, endConditions, numberOfPlayers);
		// }
	}
}
