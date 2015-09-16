using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicNestedLinkedSourcesTests {
        private FakeReferenceLoader<WithNestedPolymorphicContents, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithNestedPolymorphicContentsLinkedSource>()
                .IsRoot<string>()
                .LoadLinkNestedLinkedSource(
                    linkedSource => linkedSource.Model.ContentContextualizations,
                    linkedSource => linkedSource.Contents,
                    reference => reference.ContentType,
                    includes => includes
                        .When<PersonWithoutContextualizationLinkedSource, string>(
                            "person",
                            reference => (string)reference.Id)
                        .When<ImageWithContextualizationLinkedSource, string>(
                            "image",
                            reference => (string)reference.Id,
                            (reference, childLinkedSource) => childLinkedSource.ContentContextualization = reference)
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<WithNestedPolymorphicContents, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_NestedPolymorphicContents() {
            _fakeReferenceLoader.FixValue(
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

            var actual = _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_NestedPolymorphicContentsWithNullInReferenceIds_ShouldIgnoreNull() {
            _fakeReferenceLoader.FixValue(
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

            var actual = _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>("1");

            Assert.That(actual.Contents.Count, Is.EqualTo(2));
        }

        [Test]
        public void LoadLink_NestedPolymorphicContentsWithoutReferenceIds_ShouldLinkEmptySet() {
            _fakeReferenceLoader.FixValue(
                new WithNestedPolymorphicContents {
                    Id = "1",
                    ContentContextualizations = null
                }
            );

            var actual = _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>("1");

            Assert.That(actual.Contents, Is.Empty);
        }

        [Test]
        public void LoadLink_NestedPolymorphicContentsWithDuplicates_ShouldLinkDuplicates() {
            _fakeReferenceLoader.FixValue(
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

            var actual = _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>("1");

            var asImageIds = actual.Contents
                .Cast<ImageWithContextualizationLinkedSource>()
                .Select(image => image.Model.Id)
                .ToList();

            Assert.That(asImageIds, Is.EquivalentTo(new[] { "i1", "i1" }));
        }

        [Test]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkEmptySet() {
            _fakeReferenceLoader.FixValue(
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

            var actual = _sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>("1");

            Assert.That(actual.Contents, Is.Empty);
        }

        public class WithNestedPolymorphicContentsLinkedSource : ILinkedSource<WithNestedPolymorphicContents> {
            public WithNestedPolymorphicContents Model { get; set; }
            public List<INestedPolymorphicContentLinkedSource> Contents { get; set; }
        }

        public interface INestedPolymorphicContentLinkedSource { }

        public class ImageWithContextualizationLinkedSource : INestedPolymorphicContentLinkedSource, ILinkedSource<Image>
        {
            public Image Model { get; set; }
            public ContentContextualization ContentContextualization { get; set; }
        }

        public class PersonWithoutContextualizationLinkedSource : INestedPolymorphicContentLinkedSource, ILinkedSource<Person> {
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
