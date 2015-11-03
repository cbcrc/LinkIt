using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTree_NestedLinkedSourceTests
    {
        [Test]
        public void CreateRootReferenceTree()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.PreImageId,
                    linkedSource => linkedSource.PreImage
                )
                .LoadLinkNestedLinkedSource(
                    linkedSource => linkedSource.Model.PersonId,
                    linkedSource => linkedSource.Person
                )
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.PostImageId,
                    linkedSource => linkedSource.PostImage
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            var sut = new LoadLinkConfig(loadLinkProtocolBuilder.GetLoadLinkExpressions());

            var actual = sut.CreateRootReferenceTree<LinkedSource>();

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
