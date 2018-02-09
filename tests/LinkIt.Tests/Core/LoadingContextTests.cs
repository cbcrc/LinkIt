// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using LinkIt.Core;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LoadingContextTests
    {
        [Fact]
        public void LookupContainsIdAlreadyLoaded_ShouldIgnore()
        {
            var loookupContext = new LookupContext();
            loookupContext.AddLookupIds<Image, string>(new[] { "a", "b" });
            var dataStore = new DataStore();
            dataStore.AddReferences(new Dictionary<string, Image>
            {
                { "b", new Image { Id = "b" } }
            });

            var sut = new LoadingContext(loookupContext, dataStore);

            var imagesToLoad = sut.ReferenceIds<Image, string>();
            Assert.Contains("a", imagesToLoad);
            Assert.DoesNotContain("b", imagesToLoad);
        }
    }
}
