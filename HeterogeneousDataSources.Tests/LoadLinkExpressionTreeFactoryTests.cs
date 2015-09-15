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
                .LoadLinkNestedLinkedSource<PersonLinkedSource, Person, string>(
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
        public void Create_Root()
        {
            var node = _loadLinkExpressions[0];
            Create_Parameterizable(node);
        }

        [Test]
        public void Create_Branch() {
            var node = _loadLinkExpressions[1];
            Create_Parameterizable(node);
        }

        [Test]
        public void Create_Leaf() {
            var node = _loadLinkExpressions[2];
            Create_Parameterizable(node);
        }

        private void Create_Parameterizable(ILoadLinkExpression node)
        {
            var actual = _sut.Create(node);

            var asModelTree = actual.Projection(n => n.LinkTargetId);

            ApprovalsExt.VerifyPublicProperties(asModelTree);
        }

        public class LoadLinkExpressionTreeLinkedSource : ILinkedSource<LoadLinkExpressionTreeContent> {
            public LoadLinkExpressionTreeContent Model { get; set; }
            public PersonLinkedSource Person { get; set; }
            public SubLoadLinkExpressionTreeLinkedSource SubPerson { get; set; }
        }

        public class LoadLinkExpressionTreeContent {
            public int Id { get; set; }
            public string PersonId { get; set; }
            public SubLoadLinkExpressionTreeContent SubPerson { get; set; }
        }

        //stle: review root test
        public class SubLoadLinkExpressionTreeLinkedSource : ILinkedSource<SubLoadLinkExpressionTreeContent> {
            public SubLoadLinkExpressionTreeContent Model { get; set; }
            public PersonLinkedSource Person { get; set; }
        }

        public class SubLoadLinkExpressionTreeContent {
            public string PersonId { get; set; }
        }
    }
}
