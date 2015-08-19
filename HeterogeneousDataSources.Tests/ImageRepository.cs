using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.Tests
{
    public class ImageRepository: IReferenceLoader
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

        public List<object> LoadReferences(List<object> ids)
        {
            return GetByIds(ids.Cast<string>().ToList())
                .Cast<object>()
                .ToList();
        }
    }
}