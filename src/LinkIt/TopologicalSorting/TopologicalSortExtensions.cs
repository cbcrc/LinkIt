// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.TopologicalSorting
{
    internal static class TopologicalSortExtensions
    {
        public static List<List<Type>> GetLoadingLevels(this TopologicalSort sort)
        {
            return sort.DependencySets
                .Select(set => set
                    .Select(dep => dep.Type.ModelType)
                    .ToList())
                .ToList();
        }
    }
}
