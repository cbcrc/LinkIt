// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.PublicApi;
using LinkIt.TestHelpers;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PersonWithoutContextualizationLinkedSource : IPolymorphicSource, ILinkedSource<Person>
    {
        public Person Model { get; set; }
    }
}