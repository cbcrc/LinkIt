using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.PublicApi;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicNestedLinkedSourceTests {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
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

        [Test]
        public void LoadLink_NestedPolymorphicContent() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
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

        //stle: dry: contextualization
        [Test]
        public void LoadLink_NestedPolymorphicContentWithContextualization_ShouldInitContextualization() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
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


        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public IPolymorphicSource Content { get; set; }
        }

        public class Model {
            public string Id { get; set; }
            public PolymorphicNestedLinkedSourcesTests.ContentContextualization ContentContextualization { get; set; }
        }
    }
}
