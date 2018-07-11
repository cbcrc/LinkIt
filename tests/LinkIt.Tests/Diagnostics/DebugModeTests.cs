// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using LinkIt.ConfigBuilders;
using LinkIt.Diagnostics;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Diagnostics
{
    public class DebugModeTests
    {
        private readonly ILoadLinkProtocol _sut;

        public DebugModeTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<BlogPostLinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.AuthorId,
                    linkedSource => linkedSource.Author)
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.MediaId,
                    linkedSource => linkedSource.Media);
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);
            loadLinkProtocolBuilder.For<MediaLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            var blogPostConfig = new ReferenceTypeConfig<BlogPost, string>(
                ids => ids.Select(GetBlogPost),
                reference => reference.Id
            );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub(blogPostConfig));
        }

        [Fact]
        public async Task FromModels_DetailsAreNotEmpty()
        {
            var models = new[]
            {
                GetBlogPost("first"),
                GetBlogPost("second")
            };

            ILoadLinkDetails details = null;
            var result = await _sut.LoadLink<BlogPostLinkedSource>().EnableDebugMode(x => details = x).FromModelsAsync(models);

            Assert.Equal("FromModelsAsync", details.CallDetails.Method);
            Assert.Equal(models, details.CallDetails.Values);
            Assert.Equal(typeof(BlogPost), details.LinkedSourceModelType);
            Assert.Equal(typeof(BlogPostLinkedSource), details.LinkedSourceType);
            Assert.Equal(result, details.Result);
            Assert.NotNull(details.Took);

            Assert.NotNull(details.CurrentStep);
            Assert.Equal(3, details.Steps.Count);

            Assert.Equal(details.Steps[2], details.CurrentStep);

            var firstStep = details.Steps[0];
            Assert.Equal(1, firstStep.StepNumber);
            Assert.Null(firstStep.LoadTook);
            Assert.NotNull(firstStep.LinkTook);
            Assert.Equal(1, firstStep.References.Count);
            Assert.Equal(typeof(BlogPost), firstStep.References[0].Type);
            Assert.Empty(firstStep.References[0].Ids);
            Assert.Equal(models, firstStep.References[0].Values);
        }

        private static BlogPost GetBlogPost(string id)
        {
            return new BlogPost
            {
                Id = id,
                AuthorId = $"author-{id}",
                MediaId = id.GetHashCode(),
            };
        }

        private class BlogPostLinkedSource : ILinkedSource<BlogPost>
        {
            public BlogPost Model { get; set; }
            public PersonLinkedSource Author { get; set; }
            public MediaLinkedSource Media { get; set; }
        }

        private class BlogPost
        {
            public string Id { get; set; }
            public string AuthorId { get; set; }
            public int MediaId { get; set; }
        }
    }
}
