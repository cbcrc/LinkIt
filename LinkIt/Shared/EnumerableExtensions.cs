#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Shared
{
    internal static class EnumerableExtensions
    {
        public static List<TKey> GetNotUniqueKey<TItem, TKey>(this IEnumerable<TItem> items, Func<TItem, TKey> keySelector)
        {
            return items
                .GroupBy(keySelector)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();
        }
    }
}