using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Tests.TestHelpers
{
    public class ImageRepository
    {
        public ImageRepository(bool isConnectionOpen)
        {
            if (!isConnectionOpen) { throw new Exception("Connection was not open."); }
        }

        public List<Image> GetByIds(List<string> ids)
        {
            if (ids.Contains("person-img-666")) { throw new Exception("Cannot load person-img-666!"); }

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