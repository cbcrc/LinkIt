#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LoadLinkConfig_ByIdsTests
    {
        public LoadLinkConfig_ByIdsTests()
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
        public void LoadLinkById()
        {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById("one");

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
        public void LoadLinkById_ManyReferencesCannotBeResolved_ShouldLinkNull()
        {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById("cannot-be-resolved");

            Assert.Null(actual);
        }

        [Fact]
        public void LoadLinkById_WithNullId_ShouldLinkNull()
        {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById<string>(null);

            Assert.Null(actual);
        }

        [Fact]
        public void LoadLinkById_WithNullId_ShouldLinkNullWithoutLoading()
        {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById<string>(null);

            var loadedReferenceTypes = _referenceLoaderStub.RecordedLookupIdContexts
                .First()
                .GetReferenceTypes();

            Assert.Empty(loadedReferenceTypes);
        }

        [Fact]
        public void LoadLinkByIds()
        {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds(new List<string> { "one", "two" });

            var expected = new[]
            {
                GetExpectedPersonLinkedSource("one"),
                GetExpectedPersonLinkedSource("two"),
            };
            actual.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public void LoadLinkByIds_ManyReferencesCannotBeResolved_ShouldLinkNull()
        {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds(new List<string> { "cannot-be-resolved" });

            Assert.Empty(actual);
        }


        [Fact]
        public void LoadLinkByIds_ManyReferencesWithDuplicates_ShouldLinkDuplicates()
        {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds(new List<string> { "a", "a" });

            var linkedSourceModelIds = actual.Select(linkedSource => linkedSource.Model.Id);
            Assert.Equal(new[] { "a", "a" }, linkedSourceModelIds);

            var loadedPersonIds = _referenceLoaderStub.RecordedLookupIdContexts
                .First()
                .GetReferenceIds<Person, string>();

            Assert.Equal(new[] { "a" }, loadedPersonIds);
        }

        [Fact]
        public void LoadLinkByIds_WithListOfNulls_ShouldLinkNullWithoutLoading()
        {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds(new List<string> { null, null });

            Assert.Empty(actual);
            var loadedReferenceTypes = _referenceLoaderStub.RecordedLookupIdContexts
                .First()
                .GetReferenceTypes();
            Assert.Empty(loadedReferenceTypes);
        }

        [Fact]
        public void LoadLinkByIds_WithNullInReferenceIds_ShouldLinkNull()
        {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds(new List<string> { "one", null, "two" });

            Assert.Equal(
                new List<string> { "one", "two" },
                actual.Select(personLinkSource => personLinkSource.Model.Id).ToList()
            );
        }
    }
}