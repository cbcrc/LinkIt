// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.PublicApi;

namespace LinkIt.TestHelpers
{
    public class PersonLinkedSource : ILinkedSource<Person>
    {
        public Image SummaryImage { get; set; }
        public Person Model { get; set; }
    }
}