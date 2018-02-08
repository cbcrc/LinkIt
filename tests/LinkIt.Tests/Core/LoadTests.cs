// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LoadTests
    {
        public LoadTests()
        {
            _referenceLoaderStub = new ReferenceLoaderStub();
            _sut = new LoadLinkProtocolBuilder().Build(() => _referenceLoaderStub);
        }

        private readonly ReferenceLoaderStub _referenceLoaderStub;
        private readonly ILoadLinkProtocol _sut;


        [Fact]
        public async Task LoadAsync()
        {
            var actual = await _sut.Load<Person>().ByIdAsync("one");

            var expected = GetExpectedPerson("one");
            actual.Should().BeEquivalentTo(expected);
        }

        private static Person GetExpectedPerson(string id)
        {
            return new Person
            {
                Id = id,
                Name = $"name-{id}",
                SummaryImageId = $"person-img-{id}"
            };
        }

        [Fact]
        public async Task Load_ReferenceCannotBeResolved_ShouldReturnNull()
        {
            var actual = await _sut.Load<Person>().ByIdAsync("cannot-be-resolved");

            Assert.Null(actual);
        }

        [Fact]
        public async Task LoadMany()
        {
            var actual = await _sut.Load<Person>().ByIdsAsync(new List<string> { "one", "two" });

            var expected = new[]
            {
                GetExpectedPerson("one"),
                GetExpectedPerson("two"),
            };
            actual.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public async Task Loads_ManyReferencesCannotBeResolved_ShouldNotReturnResults()
        {
            var actual = await _sut.Load<Person>().ByIdsAsync(new List<string> { "cannot-be-resolved" });

            Assert.Empty(actual);
        }


        [Fact]
        public async Task Loads_ManyReferencesWithDuplicates_ShouldLoadOnlyOne()
        {
            var actual = await _sut.Load<Person>().ByIdsAsync(new [] { "a", "a" });

            var personIds = actual.Select(person => person.Id);
            Assert.Equal(new[] { "a", "a" }, personIds);

            var loadedPersonIds = _referenceLoaderStub.RecordedLookupIdContexts
                .First()
                .ReferenceIds<Person, string>();

            Assert.Equal(new[] { "a" }, loadedPersonIds);
        }

        [Fact]
        public async Task Loads_WithListOfNulls_ShouldNotCallReferenceLoader()
        {
            var actual = await _sut.Load<Person>().ByIdsAsync(new List<string> { null, null });

            Assert.Empty(actual);
            Assert.Empty(_referenceLoaderStub.RecordedLookupIdContexts);
        }

        [Fact]
        public async Task Loads_WithNullInReferenceIds_ShouldLoadAsync()
        {
            var actual = await _sut.Load<Person>().ByIdsAsync(new List<string> { "one", null, "two" });

            Assert.Equal(
                new List<string> { "one", "two" },
                actual.Select(person => person.Id).ToList()
            );
        }
    }
}