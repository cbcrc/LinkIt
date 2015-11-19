using System.Collections.Generic;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicList_WithDependenciesBetweenItemsTests {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLinkForList(
                    linkedSource => linkedSource.Model.PolyLinks,
                    linkedSource => linkedSource.PolyLinks,
                    link => link.Type,
                    includes => includes
                        .Include<Image>().AsReferenceById(
                            "image",
                            link=>link.Id
                        )
                        .Include<PersonLinkedSource>().AsNestedLinkedSourceById(
                            "person",
                            link => link.Id
                        )
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            
            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_WithDependenciesBetweenItems_ShouldLink3Items() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
                    Id = "1",
                    PolyLinks = new List<Link>{
                        new Link{ Type = "image", Id = "before"},
                        new Link{ Type = "person", Id = "the-person"},
                        new Link{ Type = "image", Id = "after"}
                    }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public List<object> PolyLinks { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public List<Link> PolyLinks { get; set; }
        }

        public class Link{
            public string Type { get; set; }
            public string Id { get; set; }
        }
    }
}
