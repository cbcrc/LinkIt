// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace LinkIt.Shared
{
    internal static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary == null || key == null)
            {
                return defaultValue;
            }

            return dictionary.ContainsKey(key)
                ? dictionary[key]
                : defaultValue;
        }
    }
}