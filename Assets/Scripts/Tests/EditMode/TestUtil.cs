using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public static class TestUtil
{
	public static (bool areEqual, string failMessage) AreArraysEqual<T>(IEnumerable<T> actual, IEnumerable<T> expected)
	{
		var failMessage = "";	
		var expectedArr = expected.OrderBy(x => x).ToArray();
		var actualArr = actual.OrderBy(x => x).ToArray();
		
		if (expectedArr.Length != actualArr.Length)
		{
			failMessage =
					$"Arrays have different lengths. Expected array length: {expectedArr.Length}, Actual array length: {actualArr.Length}.\nExpected array: {string.Join(", ", expectedArr)}.\nActual array: {string.Join(", ", actualArr)}";
			return (false, failMessage);
		}

		for (var i = 0; i < expectedArr.Length; i++)
		{
			if (expectedArr[i].Equals(actualArr[i]))
			{
				continue;
			}

			failMessage = $"Arrays differ at index {i}. Expected value: {expectedArr[i]}, Actual value: {actualArr[i]}";
			return (false, failMessage);
		}
		
		return (true, failMessage);
	}
	
	public static void AssertArraysAreEqual<T>(IEnumerable<T> actual, IEnumerable<T> expected)
	{
		var (isSuccess, failMessage) = AreArraysEqual(actual, expected);
		Assert.IsTrue(isSuccess, failMessage);
		
	}
}