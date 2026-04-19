namespace Chess
{
	public enum EndConditionType
	{
		CheckMate = 0,
		StaleMate = 1
	}

	public enum Result
	{
		Win = 0,
		Draw = 1,
		Loose = 2
	}

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
