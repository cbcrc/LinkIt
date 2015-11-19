using System.Collections.Generic;
using System.Linq;

namespace LinkIt.Tests.Shared
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