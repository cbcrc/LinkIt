using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.Tests.Shared
{
    public class ImageRepository
    {
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