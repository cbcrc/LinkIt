using System.Collections.Generic;
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
                        linkedSource => linkedSource.Model.ContentContextualizations,
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
        public void LoadLink_WithNestedPolymorphicContents() {
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
