// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PolymorphicSubLinkedSourceTests
    {
        private ILoadLinkProtocol _sut;

        public PolymorphicSubLinkedSourceTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkPolymorphic(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes.Include<PdfReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                            "pdf",
                            link => link,
                            (linkedSource, referenceIndex, childLinkedSource) =>
                                childLinkedSource.Contextualization = "From the level below:" + linkedSource.Model.Id
                        )
                        .Include<WebPageReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                            "web-page",
                            link => link.GetAsBlogPostReference()
                        )
                );

            loadLinkProtocolBuilder.For<WebPageReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ImageId,
                    linkedSource => linkedSource.Image);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_PolymorphicSubLinkedSourceWithoutReferencesAsync()
        {
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(
                new Model
                {
                    Id = "1",
                    Target = new PolymorphicReference
                    {
                        Type = "pdf",
                        Id = "a"
                    }
                }
            );

            var linkedSource = Assert.IsType<PdfReferenceLinkedSource>(actual.Target);
            Assert.Equal("From the level below:1", linkedSource.Contextualization);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_PolymorphicSubLinkedSourceWithGetSubLinkedSourceModelAsync()
        {
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(
                new Model
                {
                    Id = "1",
                    Target = new PolymorphicReference
                    {
                        Type = "web-page",
                        Id = "a"
                    }
                }
            );

            var linkedSource = Assert.IsType<WebPageReferenceLinkedSource>(actual.Target);
            Assert.Equal("title-a", linkedSource.Model.Title);
            Assert.Equal("computed-image-ida", linkedSource.Model.ImageId);
            Assert.Equal("computed-image-ida", linkedSource.Image.Id);
        }


        public class LinkedSource : ILinkedSource<Model>
        {
            public object Target { get; set; }
            public Model Model { get; set; }
        }

        public class WebPageReferenceLinkedSource : ILinkedSource<WebPageReference>
        {
            public Image Image { get; set; }
            public WebPageReference Model { get; set; }
        }

        public class PdfReferenceLinkedSource : ILinkedSource<PolymorphicReference>
        {
            public string Contextualization { get; set; }
            public PolymorphicReference Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public PolymorphicReference Target { get; set; }
        }

        public class PolymorphicReference
        {
            public string Type { get; set; }
            public string Id { get; set; }

            public WebPageReference GetAsBlogPostReference()
            {
                return new WebPageReference
                {
                    Title = "title-" + Id,
                    ImageId = "computed-image-id" + Id
                };
            }
        }

        public class WebPageReference
        {
            public string Title { get; set; }
            public string ImageId { get; set; }
        }
    }
}