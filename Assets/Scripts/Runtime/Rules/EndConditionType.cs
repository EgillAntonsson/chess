
using System;
using Codice.Client.BaseCommands.BranchExplorer.Layout;

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

	public struct EndCondition
	{
		public EndConditionType Type { get; }
		public Result PlayerThatMovedResult { get; }

		public EndCondition(EndConditionType type, Result playerThatMovedResult)
		{
			Type = type;
			PlayerThatMovedResult = playerThatMovedResult;
		}

		public Result GetOpponentsResult()
		{
			switch (PlayerThatMovedResult)
			{
				case Result.Win:
					return Result.Loose;
				case Result.Draw:
					return Result.Draw;
				case Result.Loose:
				default:
					return Result.Draw;
			}
		}
	}
}