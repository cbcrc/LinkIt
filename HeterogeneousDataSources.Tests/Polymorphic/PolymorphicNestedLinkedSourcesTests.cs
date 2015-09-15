using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.LoadLinkExpressions.Polymorphic;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicNestedLinkedSourcesTests {

        private LoadLinkProtocolFactory<WithNestedPolymorphicContents, string> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp()
        {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<WithNestedPolymorphicContents, string>(
                loadLinkExpressions: new List<ILoadLinkExpression> {
                    new RootLoadLinkExpression<WithNestedPolymorphicContentsLinkedSource, WithNestedPolymorphicContents, string>(),
                    new PolymorphicNestedLinkedSourcesLoadLinkExpression<WithNestedPolymorphicContentsLinkedSource, INestedPolymorphicContentLinkedSource, ContentContextualization, string>(
                        "temp: until protocol builder",
                        linkedSource => linkedSource.Model.ContentContextualizations,
                        linkedSource => linkedSource.Contents, 
                        (linkedSource, childLinkedSource) => linkedSource.Contents = childLinkedSource,
                        link => link.ContentType,
                        new Dictionary<
                            string, 
                            IPolymorphicNestedLinkedSourceInclude<INestedPolymorphicContentLinkedSource, ContentContextualization>
                        >{
                            {
                                "person", 
                                new PolymorphicNestedLinkedSourceInclude<
                                    INestedPolymorphicContentLinkedSource,
                                    ContentContextualization,
                                    PersonWithoutContextualizationLinkedSource,
                                    Person,
                                    string
                                >(
                                    link => (string) link.Id
                                )
                            },
                            {
                                "image",
                                new PolymorphicNestedLinkedSourceInclude<
                                    INestedPolymorphicContentLinkedSource,
                                    ContentContextualization,
                                    ImageWithContextualizationLinkedSource,
                                    Image,
                                    string
                                >(
                                    link => (string)link.Id,
                                    (link,childLinkedSource) => childLinkedSource.ContentContextualization = link
                                ) 
                            }
                        }
                    ),
                },
                getReferenceIdFunc: reference => reference.Id
            );
        }

        [Test]
        public void LoadLink_NestedPolymorphicContents() {
            var sut = _loadLinkProtocolFactory.Create(
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

            var actual = sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_NestedPolymorphicContentsWithNullInReferenceIds_ShouldIgnoreNull() {
            var sut = _loadLinkProtocolFactory.Create(
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

            var actual = sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>("1");

            Assert.That(actual.Contents.Count, Is.EqualTo(2));
        }

        [Test]
        public void LoadLink_NestedPolymorphicContentsWithoutReferenceIds_ShouldLinkEmptySet() {
            var sut = _loadLinkProtocolFactory.Create(
                new WithNestedPolymorphicContents {
                    Id = "1",
                    ContentContextualizations = null
                }
            );

            var actual = sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>("1");

            Assert.That(actual.Contents, Is.Empty);
        }

        [Test]
        public void LoadLink_NestedPolymorphicContentsWithDuplicates_ShouldLinkDuplicates() {
            var sut = _loadLinkProtocolFactory.Create(
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

            var actual = sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>("1");

            var asImageIds = actual.Contents
                .Cast<ImageWithContextualizationLinkedSource>()
                .Select(image => image.Model.Id)
                .ToList();

            Assert.That(asImageIds, Is.EquivalentTo(new[] { "i1", "i1" }));
        }

        [Test]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkEmptySet() {
            var sut = _loadLinkProtocolFactory.Create(
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

            var actual = sut.LoadLink<WithNestedPolymorphicContentsLinkedSource>("1");

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
