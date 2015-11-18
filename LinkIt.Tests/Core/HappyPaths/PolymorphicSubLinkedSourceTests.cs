using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicSubLinkedSourceTests {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes
                        .Include<PdfReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                            "pdf",
                            link=>link
                        )
                        .Include<WebPageReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                            "web-page",
                            link=>link.GetAsBlogPostReference()
                        )
                );

            loadLinkProtocolBuilder.For<WebPageReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ImageId,
                    linkedSource => linkedSource.Image
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_PolymorphicSubLinkedSourceWithoutReferences() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
                    Id = "1",
                    Target = new PolymorphicReference {
                        Type = "pdf",
                        Id = "a"
                    }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_PolymorphicSubLinkedSourceWithGetSubLinkedSourceModel() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
                    Id = "1",
                    Target = new PolymorphicReference {
                        Type = "web-page",
                        Id = "a"
                    }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }


        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public object Target { get; set; }
        }

        public class WebPageReferenceLinkedSource : ILinkedSource<WebPageReference>
        {
            public WebPageReference Model { get; set; }
            public Image Image { get; set; }
        }

        public class PdfReferenceLinkedSource : ILinkedSource<PolymorphicReference> {
            public PolymorphicReference Model { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public PolymorphicReference Target { get; set; }
        }

        public class PolymorphicReference {
            public string Type { get; set; }
            public string Id { get; set; }

            public WebPageReference GetAsBlogPostReference()
            {
                return new WebPageReference{
                    Title = "title-" + Id,
                    ImageId = "computed-image-id" + Id,
                };
            }
        }

        public class WebPageReference{
            public string Title { get; set; }
            public string ImageId{ get; set; }
        }
    }
}
