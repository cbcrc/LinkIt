#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;


namespace LinkIt.Tests.Core.Polymorphic {
    public class PolymorphicNestedLinkedSourceTests {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithNestedPolymorphicContentLinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.ContentContextualization,
                    linkedSource => linkedSource.Content,
                    link => link.ContentType,
                    includes => includes
                        .Include<PolymorphicNestedLinkedSourcesTests.PersonWithoutContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            "person",
                            link => (string)link.Id)
                        .Include<PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            "image",
                            link => (string)link.Id,
                            (linkedSource, referenceIndex, childLinkedSource) =>
                                childLinkedSource.ContentContextualization = linkedSource.Model.ContentContextualization)
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public void LoadLink_NestedPolymorphicContent() {
            var actual = _sut.LoadLink<WithNestedPolymorphicContentLinkedSource>().FromModel(
                new WithNestedPolymorphicContent {
                    Id = "1",
                    ContentContextualization = new PolymorphicNestedLinkedSourcesTests.ContentContextualization {
                        ContentType = "person",
                        Id = "p1",
                        Title = "altered person title"
                    }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Fact]
        public void LoadLink_NestedPolymorphicContentWithContextualization_ShouldInitContextualization() {
            var actual = _sut.LoadLink<WithNestedPolymorphicContentLinkedSource>().FromModel(
                new WithNestedPolymorphicContent {
                    Id = "1",
                    ContentContextualization = new PolymorphicNestedLinkedSourcesTests.ContentContextualization {
                        ContentType = "image",
                        Id = "i1",
                        Title = "altered image title"
                    }
                }
            );

            var contentAsImage =
                actual.Content as PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource;
            Assert.That(contentAsImage.ContentContextualization.Title, Is.EqualTo("altered image title"));
        }


        public class WithNestedPolymorphicContentLinkedSource : ILinkedSource<WithNestedPolymorphicContent> {
            public WithNestedPolymorphicContent Model { get; set; }
            public IPolymorphicSource Content { get; set; }
        }

        public class WithNestedPolymorphicContent {
            public string Id { get; set; }
            public PolymorphicNestedLinkedSourcesTests.ContentContextualization ContentContextualization { get; set; }
        }
    }
}
