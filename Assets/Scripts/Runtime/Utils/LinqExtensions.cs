using System.Collections.Generic;
using System.Linq;

public static class LinqExtensions
{
	public static T[,] ToRectangularArray<T>(this IEnumerable<IEnumerable<T>> enumerables)
	{
		var arrays = enumerables as IEnumerable<T>[] ?? enumerables.ToArray();
		int rows = arrays.Count();
		int cols = arrays.First().Count();
		var ret = new T[rows, cols];

		for (int i = 0; i < cols; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				ret[j, i] = arrays.ElementAt(i).ElementAt(j);
			}
		}

		return ret;
	}
}
