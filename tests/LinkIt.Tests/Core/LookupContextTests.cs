// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Linq;
using LinkIt.Core;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LookupContextTests
    {
        private readonly LookupContext _sut;

        public LookupContextTests()
        {
            _sut = new LookupContext();
        }

        [Fact]
        public void Add_Distinct_ShouldAdd()
        {
            _sut.AddLookupId<Image, string>("a");
            _sut.AddLookupId<Image, string>("b");

            Assert.Equal(new[] { "a", "b" }, _sut.LookupIds[typeof(Image)].Cast<string>());
        }

        [Fact]
        public void Add_WithDuplicates_DuplicatesShouldNotBeAdded()
        {
            _sut.AddLookupId<Image, string>("a");
            _sut.AddLookupId<Image, string>("a");
            _sut.AddLookupId<Image, string>("b");

            Assert.Equal(new[] { "a", "b" }, _sut.LookupIds[typeof(Image)].Cast<string>());
        }

        [Fact]
        public void Add_NullId_ShouldIgnoreNullId()
        {
            _sut.AddLookupId<Image, string>(null);

            Assert.Empty(_sut.LookupIds.Keys);
        }
    }
}