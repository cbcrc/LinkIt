// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class ManyReferencesTests
    {
        private ILoadLinkProtocol _sut;

        public ManyReferencesTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<ManyReferencesLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage)
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.AuthorImageId,
                    linkedSource => linkedSource.AuthorImage)
                .LoadLinkReferencesByIds(
                    linkedSource => linkedSource.Model.FavoriteImageIds,
                    linkedSource => linkedSource.FavoriteImages);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ManyReferencesAsync()
        {
            var actual = await _sut.LoadLink<ManyReferencesLinkedSource>().FromModelAsync(
                new ManyReferencesContent
                {
                    Id = 1,
                    SummaryImageId = "summary-image-id",
                    AuthorImageId = "author-image-id",
                    FavoriteImageIds = new [] { "one", "two" }
                }
            );

            Assert.Collection(
                actual.FavoriteImages,
                image =>
                {
                    Assert.Equal("one", image.Id);
                    Assert.Equal("alt-one", image.Alt);
                },
                image =>
                {
                    Assert.Equal("two", image.Id);
                    Assert.Equal("alt-two", image.Alt);
                }
            );
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ManyReferencesWithNullInReferenceIds_ShouldLinkNullAsync()
        {
            var actual = await _sut.LoadLink<ManyReferencesLinkedSource>().FromModelAsync(
                new ManyReferencesContent
                {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = new [] { "one", null, "two" }
                }
            );

            Assert.Equal(new [] { "one", "two" }, actual.FavoriteImages.Select(image => image.Id));
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySetAsync()
        {
            var actual = await _sut.LoadLink<ManyReferencesLinkedSource>().FromModelAsync(
                new ManyReferencesContent
                {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = null
                }
            );

            Assert.Empty(actual.FavoriteImages);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicatesAsync()
        {
            var actual = await _sut.LoadLink<ManyReferencesLinkedSource>().FromModelAsync(
                new ManyReferencesContent
                {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = new [] { "a", "a" }
                }
            );

            var linkedImagesIds = actual.FavoriteImages.Select(image => image.Id);
            Assert.Equal(new[] { "a", "a" }, linkedImagesIds);
        }


        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ManyReferencesCannotBeResolved_ShouldLinkNullAsync()
        {
            var actual = await _sut.LoadLink<ManyReferencesLinkedSource>().FromModelAsync(
                new ManyReferencesContent
                {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = new [] { "cannot-be-resolved", "cannot-be-resolved" }
                }
            );

            Assert.Empty(actual.FavoriteImages);
        }
    }

    public class ManyReferencesLinkedSource : ILinkedSource<ManyReferencesContent>
    {
        public Image SummaryImage { get; set; }
        public Image AuthorImage { get; set; }
        public List<Image> FavoriteImages { get; set; }
        public ManyReferencesContent Model { get; set; }
    }

    public class ManyReferencesContent
    {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
        public string AuthorImageId { get; set; }
        public string[] FavoriteImageIds { get; set; }
    }
}