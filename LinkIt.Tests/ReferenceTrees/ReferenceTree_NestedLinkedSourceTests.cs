using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Core;
using LinkIt.PublicApi;
using LinkIt.PublicApi;
using LinkIt.ReferenceTrees;
using LinkIt.Tests.Core;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.ReferenceTrees {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTree_NestedLinkedSourceTests
    {
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PreImageId,
                    linkedSource => linkedSource.PreImage
                )
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.PersonId,
                    linkedSource => linkedSource.Person
                )
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PostImageId,
                    linkedSource => linkedSource.PostImage
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            _sut = (LoadLinkProtocol)loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void CreateRootReferenceTree() {
            var actual = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void ParseLoadingLevels() {
            var rootReferenceTree = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            var actual = rootReferenceTree.ParseLoadingLevels();

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public Image PreImage { get; set; }
            public PersonLinkedSource Person { get; set; }
            public Image PostImage { get; set; }
        }

        public class Model {
            public int Id { get; set; }
            public string PreImageId { get; set; }
            public string PersonId { get; set; }
            public string PostImageId { get; set; }
        }
    }
}
