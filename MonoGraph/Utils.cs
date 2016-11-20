using System;
using System.Collections.Generic;
            
namespace MonoGraph
{
	public struct IndexPair<TValue>
	{
		public IndexPair(int index, TValue value)
		{
			Index = index;
			Value = value;
		}

		public int Index { get; }
		public TValue Value { get; }
	}

	public static class Utils
	{
		public static bool ContainsByKey<T>(IEnumerable<T> enumerable, Func<T, bool> action)
		{
			foreach (var item in enumerable)
			{
				if (action(item))
				{
					return true;
				}
			}

			return false;
		}

		// Returns index of item or -1
		public static int FindByKey<T>(IList<T> enumerable, Func<T, bool> action)
		{
			foreach (var indexedValue in Enumerate(enumerable))
			{
				if (action(indexedValue.Value))
				{
					return indexedValue.Index;
				}
			}

			return -1;
		}

		public static IEnumerable<IndexPair<T>> Enumerate<T>(this IEnumerable<T> enumerable)
		{
			var i = 0;
			foreach (var item in enumerable)
			{
				yield return new IndexPair<T>(i, item);
			}
		}
	}
}
