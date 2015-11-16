using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.Protocols;
using LinkIt.ReferenceTrees;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.ReferenceTrees {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTree_PolymorphicTests
    {
        private LoadLinkConfig _sut;

        [SetUp]
        public void SetUp(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.PolyRef,
                    linkedSource => linkedSource.Poly,
                    link => link.Kind,
                    includes => includes
                        .Include<PersonLinkedSource>().AsNestedLinkedSourceById(
                            "person-nested",
                            link => (string)link.Value
                        )
                        .Include<PersonLinkedSource>().AsNestedLinkedSourceFromModel(
                            "person-sub",
                            link => (Person)link.Value
                        )
                        .Include<Image>().AsReferenceById(
                            "img",
                            link=>(string)link.Value
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
        public void CreateRootReferenceTree() {
            var actual = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void ParseLoadingLevels() {
            var rootReferenceTree = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            var actual = rootReferenceTree.ParseLoadLevels();

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public object Poly { get; set; }
        }

        public class Model {
            public int Id { get; set; }
            public PolymorphicRef PolyRef { get; set; }
        }

        public class PolymorphicRef
        {
            public string Kind { get; set; }
            public object Value { get; set; }
        }
    }
}
