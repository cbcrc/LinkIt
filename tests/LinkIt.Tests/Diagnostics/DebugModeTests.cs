// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
        public async Task FromModels_DetailsAreComplete()
        {
            // Arrange
            var models = new[]
            {
                GetBlogPost("first"),
                GetBlogPost("second")
            };

            // Act
            ILoadLinkDetails details = null;
            var result = await _sut.LoadLink<BlogPostLinkedSource>().EnableDebugMode(x => details = x).FromModelsAsync(models);

            // Assert
            details.CallDetails.Method.Should().Be("FromModelsAsync");
            details.CallDetails.Values.Should().Equal(models);

            details.LinkedSourceModelType.Should().Be<BlogPost>();
            details.LinkedSourceType.Should().Be<BlogPostLinkedSource>();

            details.Result.Should().BeEquivalentTo(result);

            details.Took.Should().NotBeNull();

            details.Steps.Should().HaveCount(3);
            details.CurrentStep.Should().Be(details.Steps[2]);

            VerifyFirstStep(details.Steps[0], models);
            VerifySecondStep(details.Steps[1], models);
            VerifyThirdStep(details.Steps[2], models);
        }

        private void VerifyFirstStep(LoadLinkStepDetails step, BlogPost[] models)
        {
            step.StepNumber.Should().Be(1);
            step.LoadTook.Should().BeNull("Models are pre-loaded");
            step.LinkTook.Should().NotBeNull();

            var reference = new ReferenceLoadDetails(typeof(BlogPost));
            reference.AddValues(models);

            step.References.Should().BeEquivalentTo(reference);
        }

        private void VerifySecondStep(LoadLinkStepDetails step, BlogPost[] models)
        {
            step.StepNumber.Should().Be(2);
            step.LoadTook.Should().NotBeNull();
            step.LinkTook.Should().NotBeNull();

            step.References.Should().HaveCount(2);

            var mediaReference = step.References.Should().Contain(item => item.Type == typeof(Media)).Subject;
            mediaReference.Ids.Should().HaveCount(2)
                .And.BeEquivalentTo(models.Select(m => m.MediaId));
            mediaReference.Values.Should().HaveCount(2)
                .And.BeEquivalentTo(models.Select(m => new { Id = m.MediaId }), o => o.ExcludingMissingMembers());

            var personReference = step.References.Should().Contain(item => item.Type == typeof(Person)).Subject;
            personReference.Ids.Should().HaveCount(2)
                .And.BeEquivalentTo(models.Select(m => m.AuthorId));
            personReference.Values.Should().HaveCount(2)
                .And.BeEquivalentTo(models.Select(m => new { Id = m.AuthorId }), o => o.ExcludingMissingMembers());
        }

        private void VerifyThirdStep(LoadLinkStepDetails step, BlogPost[] models)
        {
            step.StepNumber.Should().Be(3);
            step.LoadTook.Should().NotBeNull();
            step.LinkTook.Should().NotBeNull();

            step.References.Should().HaveCount(1);

            var reference = step.References.Should().Contain(item => item.Type == typeof(Image)).Subject;
            reference.Ids.Should().HaveCount(4);
            reference.Values.Should().HaveCount(4);
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
