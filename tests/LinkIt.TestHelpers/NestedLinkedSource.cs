// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.PublicApi;

namespace LinkIt.TestHelpers
{
    public class NestedLinkedSource : ILinkedSource<NestedContent>
    {
        public PersonLinkedSource AuthorDetail { get; set; }
        public Person ClientSummary { get; set; }
        public NestedContent Model { get; set; }
    }
}
