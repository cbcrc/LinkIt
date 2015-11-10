using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    public static class EnumerableExtensions
    {
        public static List<TKey> GetNotUniqueKey<TItem, TKey>(this IEnumerable<TItem> items, Func<TItem, TKey> keySelector) {
            return items
                .GroupBy(keySelector)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();
        }
    }
}