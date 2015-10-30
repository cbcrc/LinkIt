using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.Tests.Shared
{
    public class MediaRepository
    {
        public List<Media> GetByIds(List<int> ids)
        {
            return ids
                .Select(id => new Media{
                    Id = id, 
                    Title = "title-" + id
                })
                .ToList();
        }
    }
}