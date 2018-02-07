// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace LinkIt.TestHelpers
{
    public class ImageRepository
    {
        public List<Image> GetByIds(IEnumerable<string> ids)
        {
            return ids
                .Where(id => id != "cannot-be-resolved")
                .Select(id => new Image{
                    Id = id,
                    Alt = "alt-" + id
                })
                .ToList();
        }
    }
}