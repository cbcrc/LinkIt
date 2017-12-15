#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Linq;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class NestedLinkedSourcesTests
    {
        private ILoadLinkProtocol _sut;

        public NestedLinkedSourcesTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<NestedLinkedSources>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.AuthorIds,
                    linkedSource => linkedSource.Authors);
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public void LoadLink_NestedLinkedSource()
        {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents
                {
                    Id = 1,
                    AuthorIds = new List<string> { "a", "b" }
                }
            );

            Assert.Collection(
                actual.Authors,
                author =>
                {
                    Assert.Equal("a", author.Model.Id);
                    Assert.Equal(author.Model.SummaryImageId, author.SummaryImage.Id);
                },
                author =>
                {
                    Assert.Equal("b", author.Model.Id);
                    Assert.Equal(author.Model.SummaryImageId, author.SummaryImage.Id);
                }
            );
        }

        [Fact]
        public void LoadLink_ManyReferencesWithNullInReferenceIds_ShouldLinkNull()
        {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents
                {
                    Id = 1,
                    AuthorIds = new List<string> { "a", null, "b" }
                }
            );

            Assert.Equal(new [] {"a", "b"}, actual.Authors.Select(a => a.Model.Id));
        }

        [Fact]
        public void LoadLink_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySet()
        {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents
                {
                    Id = 1,
                    AuthorIds = null
                }
            );

            Assert.Empty(actual.Authors);
        }

        [Fact]
        public void LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicates()
        {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents
                {
                    Id = 1,
                    AuthorIds = new List<string> { "a", "a" }
                }
            );

            var linkedAuthroIds = actual.Authors.Select(author => author.Model.Id);
            Assert.Equal(new[] { "a", "a" }, linkedAuthroIds);

            Assert.Equal(2, actual.Authors.Count);
        }

        [Fact]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkNull()
        {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents
                {
                    Id = 1,
                    AuthorIds = new List<string> { "cannot-be-resolved", "cannot-be-resolved" }
                }
            );

            Assert.Empty(actual.Authors);
        }
    }

    public class NestedLinkedSources : ILinkedSource<NestedContents>
    {
        public List<PersonLinkedSource> Authors { get; set; }
        public NestedContents Model { get; set; }
    }

    public class NestedContents
    {
        public int Id { get; set; }
        public List<string> AuthorIds { get; set; }
    }
}