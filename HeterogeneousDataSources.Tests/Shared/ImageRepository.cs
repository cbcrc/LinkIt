using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.Tests.Shared
{
    public class ImageRepository
    {
        public ImageRepository(bool isConnectionOpen)
        {
            if (!isConnectionOpen) { throw new Exception("Connection was not open."); }
        }

        public List<Image> GetByIds(List<string> ids)
        {
            return ids
                .Select(id => new Image{
                    Id = id, 
                    Alt = "alt-" + id
                })
                .ToList();
        }
    }
}