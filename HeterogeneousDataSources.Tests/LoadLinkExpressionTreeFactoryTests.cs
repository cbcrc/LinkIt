using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkExpressionTreeFactoryTests
    {
        private LoadLinkExpressionTreeFactory _sut;
        private List<ILoadLinkExpression> _loadLinkExpressions;

        [SetUp]
        public void SetUp(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LoadLinkExpressionTreeLinkedSource>()
                .IsRoot<int>()
                .LoadLinkNestedLinkedSource(
                    linkedSource => linkedSource.Model.PersonId,
                    linkedSource => linkedSource.Person
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _loadLinkExpressions = loadLinkProtocolBuilder.GetLoadLinkExpressions();
            _sut = new LoadLinkExpressionTreeFactory(_loadLinkExpressions);
        }

        [Test]
        public void Create()
        {
            var actual = _sut.Create(_loadLinkExpressions.First());

            var asModelTree = actual.Projection(n => n.LinkTargetId);

            ApprovalsExt.VerifyPublicProperties(asModelTree);
        }

        public class LoadLinkExpressionTreeLinkedSource : ILinkedSource<LoadLinkExpressionTreeContent> {
            public LoadLinkExpressionTreeContent Model { get; set; }
            public PersonLinkedSource Person { get; set; }
        }

        public class LoadLinkExpressionTreeContent {
            public int Id { get; set; }
            public string PersonId { get; set; }
        }
    }
}
