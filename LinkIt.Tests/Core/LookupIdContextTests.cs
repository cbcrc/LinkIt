#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using LinkIt.Core;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LookupIdContextTests
    {
        private LookupIdContext _sut;

        public LookupIdContextTests()
        {
            _sut = new LookupIdContext();
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