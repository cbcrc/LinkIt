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
    public class ReferenceTree_ReferenceTests
    {
        private LoadLinkConfig _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PersonOneId,
                    linkedSource => linkedSource.PersonOne
                )
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PersonTwoId,
                    linkedSource => linkedSource.PersonTwo
                );
            _sut = new LoadLinkConfig(loadLinkProtocolBuilder.GetLoadLinkExpressions());
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
            public Person PersonOne { get; set; }
            public Person PersonTwo { get; set; }
        }

        public class Model {
            public int Id { get; set; }
            public string PersonOneId { get; set; }
            public string PersonTwoId { get; set; }
        }
    }
}
