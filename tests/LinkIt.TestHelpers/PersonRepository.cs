﻿// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace LinkIt.TestHelpers
{
    public class PersonRepository
    {
        public List<Person> GetByIds(IEnumerable<string> ids)
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