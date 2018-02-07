// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PolymorphicNestedLinkedSourceTests
    {
        private ILoadLinkProtocol _sut;

        public PolymorphicNestedLinkedSourceTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithNestedPolymorphicContentLinkedSource>()
                .LoadLinkPolymorphic(
                    linkedSource => linkedSource.Model.ContentContextualization,
                    linkedSource => linkedSource.Content,
                    link => link.ContentType,
                    includes => includes.Include<PersonWithoutContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            "person",
                            link => (string) link.Id)
                        .Include<ImageWithContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            "image",
                            link => (string) link.Id,
                            (linkedSource, referenceIndex, childLinkedSource) =>
                                childLinkedSource.ContentContextualization = linkedSource.Model.ContentContextualization)
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedPolymorphicContentAsync()
        {
            var actual = await _sut.LoadLink<WithNestedPolymorphicContentLinkedSource>().FromModelAsync(
                new WithNestedPolymorphicContent
                {
                    Id = "1",
                    ContentContextualization = new ContentContextualization
                    {
                        ContentType = "person",
                        Id = "p1",
                        Title = "altered person title"
                    }
                }
            );

            var linkedSource = Assert.IsType<PersonWithoutContextualizationLinkedSource>(actual.Content);
            Assert.Equal("p1", linkedSource.Model.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedPolymorphicContentWithContextualization_ShouldInitContextualizationAsync()
        {
            var actual = await _sut.LoadLink<WithNestedPolymorphicContentLinkedSource>().FromModelAsync(
                new WithNestedPolymorphicContent
                {
                    Id = "1",
                    ContentContextualization = new ContentContextualization
                    {
                        ContentType = "image",
                        Id = "i1",
                        Title = "altered image title"
                    }
                }
            );

            var contentAsImage =
                actual.Content as ImageWithContextualizationLinkedSource;
            Assert.Equal("altered image title", contentAsImage.ContentContextualization.Title);
        }


        public class WithNestedPolymorphicContentLinkedSource : ILinkedSource<WithNestedPolymorphicContent>
        {
            public IPolymorphicSource Content { get; set; }
            public WithNestedPolymorphicContent Model { get; set; }
        }

        public class WithNestedPolymorphicContent
        {
            public string Id { get; set; }
            public ContentContextualization ContentContextualization { get; set; }
        }
    }
}