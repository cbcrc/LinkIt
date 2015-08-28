﻿using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.Tests.Shared
{
    public class PersonRepository
    {
        public List<Person> GetByIds(List<string> ids)
        {
            return ids
                .Select(id => new Person {
                    Id = id, 
                    Name = "name-" + id,
                    SummaryImageId = "person-img-" + id
                })
                .ToList();
        }
    }
}