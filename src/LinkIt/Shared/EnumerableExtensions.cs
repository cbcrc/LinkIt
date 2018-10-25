// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Shared
{
    internal static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items?.Any() != true;
        }

        public static List<TKey> GetNotUniqueKey<TItem, TKey>(this IEnumerable<TItem> items, Func<TItem, TKey> keySelector)
        {
            return items
                .GroupBy(keySelector)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> items)
        {
            return items.Where(item => (object) item != null);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            return new HashSet<T>(items);
        }
    }
}
