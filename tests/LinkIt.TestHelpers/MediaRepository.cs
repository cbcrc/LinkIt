// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace LinkIt.TestHelpers
{
    public class MediaRepository
    {
        public List<Media> GetByIds(IEnumerable<int> ids)
        {
            return ids
                .Select(id => new Media{
                    Id = id,
                    Title = "title-" + id,
                    SummaryImageId = "img" + id
                })
                .ToList();
        }
    }
}