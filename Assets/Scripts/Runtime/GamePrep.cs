
namespace Chess
{
	public class GamePrep
	{
		public GamePrep()
		{
			var variant = VariantFactory.Create(VariantType.Standard);
			var game = new Game(variant);
		}
	}
}
