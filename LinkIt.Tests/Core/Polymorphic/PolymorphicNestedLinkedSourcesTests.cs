#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;


namespace LinkIt.Tests.Core.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicNestedLinkedSourcesTests {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithNestedPolymorphicContentsLinkedSource>()
                .PolymorphicLoadLinkForList(
                    linkedSource => linkedSource.Model.ContentContextualizations,
                    linkedSource => linkedSource.Contents,
                    link => link.ContentType,
                    includes => includes
                        .Include<PersonWithoutContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            "person",
                            link => (string)link.Id)
                        .Include<ImageWithContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            "image",
                            link => (string)link.Id,
                            (linkedSource, referenceIndex, childLinkedSource) =>
                            {
                                var contextualization = linkedSource.Model.ContentContextualizations[referenceIndex];
                                childLinkedSource.ContentContextualization = contextualization;
                            }
                        )
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_NestedPolymorphicContents() {
            var actual = _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>().FromModel(
                new WithNestedPolymorphicContents {
                    Id = "1",
                    ContentContextualizations = new List<ContentContextualization>{
                        new ContentContextualization{
                            ContentType = "person",
                            Id = "p1",
                            Title = "altered person title"
                        },
                        new ContentContextualization{
                            ContentType = "image",
                            Id = "i1",
                            Title = "altered image title"
                        }
                    }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_NestedPolymorphicContentsWithNullInReferenceIds_ShouldLinkNull() {
            var actual = _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>().FromModel(
                new WithNestedPolymorphicContents {
                    Id = "1",
                    ContentContextualizations = new List<ContentContextualization>{
                        new ContentContextualization{
                            ContentType = "person",
                            Id = "p1",
                            Title = "altered person title"
                        },
                        null,
                        new ContentContextualization{
                            ContentType = "image",
                            Id = "i1",
                            Title = "altered image title"
                        }
                    }
                }
            );

            Assert.That(actual.Contents.Count, Is.EqualTo(2));
            Assert.That(
                ((ImageWithContextualizationLinkedSource)actual.Contents[1]).ContentContextualization.Title, 
                Is.EqualTo("altered image title")
            );

        }

        [Test]
        public void LoadLink_NestedPolymorphicContentsWithoutReferenceIds_ShouldLinkEmptySet() {
            var actual = _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>().FromModel(
                new WithNestedPolymorphicContents {
                    Id = "1",
                    ContentContextualizations = null
                }
            );

            Assert.That(actual.Contents, Is.Empty);
        }

        [Test]
        public void LoadLink_NestedPolymorphicContentsWithDuplicates_ShouldLinkDuplicates() {
            var actual = _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>().FromModel(
                new WithNestedPolymorphicContents {
                    Id = "1",
                    ContentContextualizations = new List<ContentContextualization>{
                        new ContentContextualization{
                            ContentType = "image",
                            Id = "i1",
                            Title = "altered image title"
                        },
                        new ContentContextualization{
                            ContentType = "image",
                            Id = "i1",
                            Title = "altered image title"
                        }
                    }
                }
            );

            var asImageIds = actual.Contents
                .Cast<ImageWithContextualizationLinkedSource>()
                .Select(image => image.Model.Id)
                .ToList();

            Assert.That(asImageIds, Is.EquivalentTo(new[] { "i1", "i1" }));
        }

        [Test]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>().FromModel(
                new WithNestedPolymorphicContents {
                    Id = "1",
                    ContentContextualizations = new List<ContentContextualization>{
                        new ContentContextualization{
                            ContentType = "image",
                            Id = "cannot-be-resolved",
                            Title = "altered image title"
                        },
                        new ContentContextualization{
                            ContentType = "image",
                            Id = "cannot-be-resolved",
                            Title = "altered image title"
                        }
                    }
                }
            );

            Assert.That(actual.Contents, Is.Empty);
        }

        public class WithNestedPolymorphicContentsLinkedSource : ILinkedSource<WithNestedPolymorphicContents> {
            public WithNestedPolymorphicContents Model { get; set; }
            public List<IPolymorphicSource> Contents { get; set; }
        }

        public class ImageWithContextualizationLinkedSource : IPolymorphicSource, ILinkedSource<Image>
        {
            public Image Model { get; set; }
            public ContentContextualization ContentContextualization { get; set; }
        }

        public class PersonWithoutContextualizationLinkedSource : IPolymorphicSource, ILinkedSource<Person> {
            public Person Model { get; set; }
        }

        public class WithNestedPolymorphicContents {
            public string Id { get; set; }
            public List<ContentContextualization> ContentContextualizations { get; set; }
        }

        public class ContentContextualization
        {
            public string ContentType { get; set; }
            public object Id{ get; set; }
            public string Title{ get; set; }
        }
    }
}
