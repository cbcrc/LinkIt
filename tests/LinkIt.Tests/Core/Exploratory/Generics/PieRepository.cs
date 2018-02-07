// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Tests.Core.Exploratory.Generics
{
    public class PieRepository<T>
    {
        public List<Pie<T>> GetByPieContentIds(IEnumerable<string> ids)
        {
            return ids.Select(id => new Pie<T>(id))
                .ToList();
        }
    }
}