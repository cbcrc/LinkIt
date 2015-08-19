using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.Tests
{
    public class PersonRepository
    {
        public List<Person> GetByIds(List<int> ids)
        {
            return ids
                .Select(id => new Person {
                    Id = id, 
                    Name = "name-" + id
                })
                .ToList();
        }
    }
}