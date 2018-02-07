// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.Core;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LoadingContextTests
    {
        private readonly LoadingContext _sut;

        public LoadingContextTests()
        {
            _sut = new LoadingContext(null);
        }

        [Fact]
        public void Add_Distinct_ShouldAdd()
        {
            _sut.AddSingle<Image, string>("a");
            _sut.AddSingle<Image, string>("b");

            Assert.Equal(new[] { "a", "b" }, _sut.GetReferenceIds<Image, string>());
        }

        [Fact]
        public void Add_WithDuplicates_DuplicatesShouldNotBeAdded()
        {
            _sut.AddSingle<Image, string>("a");
            _sut.AddSingle<Image, string>("a");
            _sut.AddSingle<Image, string>("b");

            Assert.Equal(new[] { "a", "b" }, _sut.GetReferenceIds<Image, string>());
        }

        [Fact]
        public void Add_NullId_ShouldIgnoreNullId()
        {
            _sut.AddSingle<Image, string>(null);

            var actual = _sut.GetReferenceTypes();

            Assert.Empty(actual);
        }
    }
}