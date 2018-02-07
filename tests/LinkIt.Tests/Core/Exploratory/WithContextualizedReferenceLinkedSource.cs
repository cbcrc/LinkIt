// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.PublicApi;

namespace LinkIt.Tests.Core.Exploratory
{
    public class WithContextualizedReferenceLinkedSource : ILinkedSource<WithContextualizedReference>
    {
        public WithContextualizedReference Model { get; set; }
        public PersonContextualizedLinkedSource Person { get; set; }
    }
}