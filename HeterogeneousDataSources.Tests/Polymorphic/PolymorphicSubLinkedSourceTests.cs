using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicSubLinkedSourceTests {
        private FakeReferenceLoader<Model, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes
                        .WhenSubLinkedSource<PdfReferenceLinkedSource, PolymorphicReference>(
                            "pdf"
                        )
                        .WhenSubLinkedSource<WebPageReferenceLinkedSource, WebPageReference>(
                            "web-page",
                            link=>link.GetAsBlogPostReference()
                        )
                );

            loadLinkProtocolBuilder.For<WebPageReferenceLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.ImageId,
                    linkedSource => linkedSource.Image
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<Model, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_PolymorphicSubLinkedSourceWithoutReferences() {
            _fakeReferenceLoader.FixValue(
                new Model {
                    Id = "1",
                    Target = new PolymorphicReference{
                        Type = "pdf",
                        Id = "a"
                    }
                }
            );

            var actual = _sut.LoadLink<LinkedSource>().ById("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_PolymorphicSubLinkedSourceWithGetSubLinkedSourceModel() {
            _fakeReferenceLoader.FixValue(
                new Model {
                    Id = "1",
                    Target = new PolymorphicReference {
                        Type = "web-page",
                        Id = "a"
                    }
                }
            );

            var actual = _sut.LoadLink<LinkedSource>().ById("1");

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
