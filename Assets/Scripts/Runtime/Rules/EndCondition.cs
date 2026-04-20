namespace Chess
{
	public readonly record struct EndCondition(EndConditionType Type, Result PlayerThatMovedResult)
	{
		public Result GetOpponentsResult() =>
			PlayerThatMovedResult switch
			{
				Result.Win => Result.Loose,
				Result.Draw => Result.Draw,
				_ => Result.Draw
			};
	}
}
