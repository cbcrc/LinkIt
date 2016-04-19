#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Tests.TestHelpers
{
    public class MediaRepository
    {
        public List<Media> GetByIds(List<int> ids)
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