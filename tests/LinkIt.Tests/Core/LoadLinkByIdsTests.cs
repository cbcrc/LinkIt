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
    public class LoadLinkByIdsTests
    {
        public LoadLinkByIdsTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _referenceLoaderStub = new ReferenceLoaderStub();
            _sut = loadLinkProtocolBuilder.Build(() => _referenceLoaderStub);
        }

        private readonly ReferenceLoaderStub _referenceLoaderStub;
        private readonly ILoadLinkProtocol _sut;

        [Fact]
        public async Task LoadLinkByIdAsync()
        {
            var actual = await _sut.LoadLink<PersonLinkedSource>().ByIdAsync("one");

            var expected = GetExpectedPersonLinkedSource("one");
            actual.Should().BeEquivalentTo(expected);
        }

        private static PersonLinkedSource GetExpectedPersonLinkedSource(string id)
        {
            return new PersonLinkedSource
            {
                Model = new Person
                {
                    Id = id,
                    Name = $"name-{id}",
                    SummaryImageId = $"person-img-{id}"
                },
                SummaryImage = new Image
                {
                    Id = $"person-img-{id}",
                    Alt = $"alt-person-img-{id}"
                }
            };
        }

        [Fact]
        public async Task LoadLinkById_ManyReferencesCannotBeResolved_ShouldLinkNull()
        {
            var actual = await _sut.LoadLink<PersonLinkedSource>().ByIdAsync("cannot-be-resolved");

            Assert.Null(actual);
        }

        [Fact]
        public async Task LoadLinkById_WithNullId_ShouldLinkNull()
        {
            var actual = await _sut.LoadLink<PersonLinkedSource>().ByIdAsync<string>(null);

            Assert.Null(actual);
        }

        [Fact]
        public async Task LoadLinkById_WithNullId_ShouldLinkNullWithoutLoading()
        {
            var actual = await _sut.LoadLink<PersonLinkedSource>().ByIdAsync<string>(null);

            var loadedReferenceTypes = _referenceLoaderStub.RecordedLookupIdContexts[0]
                .ReferenceTypes;

            Assert.Empty(loadedReferenceTypes);
        }

        [Fact]
        public async Task LoadLinkByIds()
        {
            var actual = await _sut.LoadLink<PersonLinkedSource>().ByIdsAsync(new List<string> { "one", "two" });

            var expected = new[]
            {
                GetExpectedPersonLinkedSource("one"),
                GetExpectedPersonLinkedSource("two"),
            };
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task LoadLinkByIds_ManyReferencesCannotBeResolved_ShouldLinkNull()
        {
            var actual = await _sut.LoadLink<PersonLinkedSource>().ByIdsAsync(new List<string> { "cannot-be-resolved" });

            Assert.Empty(actual);
        }

        [Fact]
        public async Task LoadLinkByIds_ManyReferencesWithDuplicates_ShouldLinkDuplicatesAsync()
        {
            var actual = await _sut.LoadLink<PersonLinkedSource>().ByIdsAsync(new List<string> { "a", "a" });

            var linkedSourceModelIds = actual.Select(linkedSource => linkedSource.Model.Id);
            Assert.Equal(new[] { "a", "a" }, linkedSourceModelIds);

            var loadedPersonIds = _referenceLoaderStub.RecordedLookupIdContexts[0]
                .ReferenceIds<Person, string>();

            Assert.Equal(new[] { "a" }, loadedPersonIds);
        }

        [Fact]
        public async Task LoadLinkByIds_WithListOfNulls_ShouldLinkNullWithoutLoadingAsync()
        {
            var actual = await _sut.LoadLink<PersonLinkedSource>().ByIdsAsync(new List<string> { null, null });

            Assert.Empty(actual);
            var loadedReferenceTypes = _referenceLoaderStub.RecordedLookupIdContexts[0]
                .ReferenceTypes;
            Assert.Empty(loadedReferenceTypes);
        }

        [Fact]
        public async Task LoadLinkByIds_WithNullInReferenceIds_ShouldLinkNullAsync()
        {
            var actual = await _sut.LoadLink<PersonLinkedSource>().ByIdsAsync(new List<string> { "one", null, "two" });

            Assert.Equal(
                new List<string> { "one", "two" },
                actual.Select(personLinkSource => personLinkSource.Model.Id).ToList()
            );
        }
    }
}