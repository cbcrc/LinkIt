using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.Tests.Shared
{
    public class PersonRepository
    {
        public List<Person> GetByIds(List<string> ids)
        {
            return ids
                .Where(id => id != "cannot-be-resolved")
                .Select(id => new Person {
                    Id = id, 
                    Name = "name-" + id,
                    SummaryImageId = "person-img-" + id
                })
                .ToList();
        }
    }
}