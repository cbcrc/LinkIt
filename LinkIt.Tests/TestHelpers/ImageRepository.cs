using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Tests.TestHelpers
{
    public class ImageRepository
    {
        public List<Image> GetByIds(List<string> ids)
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