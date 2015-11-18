using System.Collections.Generic;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicNestedLinkedSourcesTests {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
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
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
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

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
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

        public class Model {
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
