﻿// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

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
        private readonly ILoadLinkProtocol _sut;

        public NestedLinkedSourcesTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<NestedLinkedSources>()
                .LoadLinkNestedLinkedSourcesByIds(
                    linkedSource => linkedSource.Model.AuthorIds,
                    linkedSource => linkedSource.Authors);
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedLinkedSourceAsync()
        {
            var actual = await _sut.LoadLink<NestedLinkedSources>().FromModelAsync(
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
        public async System.Threading.Tasks.Task LoadLink_ManyReferencesWithNullInReferenceIds_ShouldLinkNullAsync()
        {
            var actual = await _sut.LoadLink<NestedLinkedSources>().FromModelAsync(
                new NestedContents
                {
                    Id = 1,
                    AuthorIds = new List<string> { "a", null, "b" }
                }
            );

            Assert.Equal(new [] {"a", "b"}, actual.Authors.Select(a => a.Model.Id));
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySetAsync()
        {
            var actual = await _sut.LoadLink<NestedLinkedSources>().FromModelAsync(
                new NestedContents
                {
                    Id = 1,
                    AuthorIds = null
                }
            );

            Assert.Empty(actual.Authors);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicatesAsync()
        {
            var actual = await _sut.LoadLink<NestedLinkedSources>().FromModelAsync(
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
        public async System.Threading.Tasks.Task LoadLink_ManyReferencesCannotBeResolved_ShouldLinkNullAsync()
        {
            var actual = await _sut.LoadLink<NestedLinkedSources>().FromModelAsync(
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