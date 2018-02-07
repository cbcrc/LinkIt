// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PolymorphicNestedLinkedSourcesTests
    {
        private ILoadLinkProtocol _sut;

        public PolymorphicNestedLinkedSourcesTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithNestedPolymorphicContentsLinkedSource>()
                .LoadLinkPolymorphicList(
                    linkedSource => linkedSource.Model.ContentContextualizations,
                    linkedSource => linkedSource.Contents,
                    link => link.ContentType,
                    includes => includes.Include<PersonWithoutContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            "person",
                            link => (string) link.Id)
                        .Include<ImageWithContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            "image",
                            link => (string) link.Id,
                            (linkedSource, referenceIndex, childLinkedSource) =>
                            {
                                var contextualization = linkedSource.Model.ContentContextualizations[referenceIndex];
                                childLinkedSource.ContentContextualization = contextualization;
                            }
                        )
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedPolymorphicContentsAsync()
        {
            var personContextualization = new ContentContextualization
            {
                ContentType = "person",
                Id = "p1",
                Title = "altered person title"
            };
            var imageConstextualization = new ContentContextualization
            {
                ContentType = "image",
                Id = "i1",
                Title = "altered image title"
            };

            var actual = await _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>().FromModelAsync(
                new WithNestedPolymorphicContents
                {
                    Id = "1",
                    ContentContextualizations = new List<ContentContextualization>
                    {
                        personContextualization,
                        imageConstextualization
                    }
                }
            );

            Assert.Collection(
                actual.Contents,
                target =>
                {
                    var linkedSource = Assert.IsType<PersonWithoutContextualizationLinkedSource>(target);
                    Assert.Equal(personContextualization.Id, linkedSource.Model.Id);
                },
                target =>
                {
                    var linkedSource = Assert.IsType<ImageWithContextualizationLinkedSource>(target);
                    Assert.Equal(imageConstextualization.Id, linkedSource.Model.Id);
                    Assert.Same(imageConstextualization, linkedSource.ContentContextualization);
                }
            );
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedPolymorphicContentsWithNullInReferenceIds_ShouldIgnoreNullAsync()
        {
            var personContextualization = new ContentContextualization
            {
                ContentType = "person",
                Id = "p1",
                Title = "altered person title"
            };
            var imageConstextualization = new ContentContextualization
            {
                ContentType = "image",
                Id = "i1",
                Title = "altered image title"
            };
            var actual = await _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>().FromModelAsync(
                new WithNestedPolymorphicContents
                {
                    Id = "1",
                    ContentContextualizations = new List<ContentContextualization>
                    {
                        personContextualization,
                        null,
                        imageConstextualization
                    }
                }
            );

            Assert.Collection(
                actual.Contents,
                target =>
                {
                    var linkedSource = Assert.IsType<PersonWithoutContextualizationLinkedSource>(target);
                    Assert.Equal(personContextualization.Id, linkedSource.Model.Id);
                },
                target =>
                {
                    var linkedSource = Assert.IsType<ImageWithContextualizationLinkedSource>(target);
                    Assert.Equal("i1", linkedSource.Model.Id);
                    Assert.Same(imageConstextualization, linkedSource.ContentContextualization);
                }
            );
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedPolymorphicContentsWithoutReferenceIds_ShouldLinkEmptySetAsync()
        {
            var actual = await _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>().FromModelAsync(
                new WithNestedPolymorphicContents
                {
                    Id = "1",
                    ContentContextualizations = null
                }
            );

            Assert.Empty(actual.Contents);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedPolymorphicContentsWithDuplicates_ShouldLinkDuplicatesAsync()
        {
            var actual = await _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>().FromModelAsync(
                new WithNestedPolymorphicContents
                {
                    Id = "1",
                    ContentContextualizations = new List<ContentContextualization>
                    {
                        new ContentContextualization
                        {
                            ContentType = "image",
                            Id = "i1",
                            Title = "altered image title"
                        },
                        new ContentContextualization
                        {
                            ContentType = "image",
                            Id = "i2",
                            Title = "altered image title"
                        }
                    }
                }
            );

            var asImageIds = actual.Contents.Cast<ImageWithContextualizationLinkedSource>()
                .Select(image => image.Model.Id);

            Assert.Equal(new[] { "i1", "i2" }, asImageIds);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ManyReferencesCannotBeResolved_ShouldNotLinkAsync()
        {
            var actual = await _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>().FromModelAsync(
                new WithNestedPolymorphicContents
                {
                    Id = "1",
                    ContentContextualizations = new List<ContentContextualization>
                    {
                        new ContentContextualization
                        {
                            ContentType = "image",
                            Id = "cannot-be-resolved",
                            Title = "altered image title"
                        },
                        new ContentContextualization
                        {
                            ContentType = "image",
                            Id = "cannot-be-resolved",
                            Title = "altered image title"
                        }
                    }
                }
            );

            Assert.Empty(actual.Contents);
        }

        public class WithNestedPolymorphicContentsLinkedSource : ILinkedSource<WithNestedPolymorphicContents>
        {
            public List<IPolymorphicSource> Contents { get; set; }
            public WithNestedPolymorphicContents Model { get; set; }
        }

        public class WithNestedPolymorphicContents
        {
            public string Id { get; set; }
            public List<ContentContextualization> ContentContextualizations { get; set; }
        }
    }
}