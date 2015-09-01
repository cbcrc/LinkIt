using System;
using System.Collections.Generic;
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
        private ILoadLinkExpression _root;
        private ILoadLinkExpression _branch;
        private ILoadLinkExpression _leaf;
        private LoadLinkExpressionTreeFactory _sut;

        [SetUp]
        public void SetUp(){
            _root = new RootLoadLinkExpression<LoadLinkExpressionTreeLinkedSource, LoadLinkExpressionTreeContent, int>();
            _branch = new NestedLinkedSourceLoadLinkExpression<LoadLinkExpressionTreeLinkedSource, PersonLinkedSource, Person, string>(
                linkedSource => linkedSource.Model.PersonId,
                (linkedSource, childLinkedSource) => linkedSource.Person = childLinkedSource);
            _leaf = new ReferenceLoadLinkExpression<PersonLinkedSource, Image, string>(
                linkedSource => linkedSource.Model.SummaryImageId,
                (linkedSource, reference) => linkedSource.SummaryImage = reference);
            _sut = new LoadLinkExpressionTreeFactory(
                new List<ILoadLinkExpression>{
                    _root,
                    _branch,
                    new SubLinkedSourceLoadLinkExpression<LoadLinkExpressionTreeLinkedSource, SubLoadLinkExpressionTreeLinkedSource, SubLoadLinkExpressionTreeContent>(
                        linkedSource => linkedSource.Model.SubPerson,
                        (linkedSource, subLinkedSources) => linkedSource.SubPerson= subLinkedSources),
                    new NestedLinkedSourceLoadLinkExpression<SubLoadLinkExpressionTreeLinkedSource,PersonLinkedSource, Person, string>(
                        linkedSource => linkedSource.Model.PersonId,
                        (linkedSource, childLinkedSource) => linkedSource.Person = childLinkedSource),
                    _leaf
                }
            );
        }

        [Test]
        public void Create_Root()
        {
            Create_Parameterizable(_root);
        }

        [Test]
        public void Create_Branch() {
            Create_Parameterizable(_branch);
        }

        [Test]
        public void Create_Leaf() {
            Create_Parameterizable(_leaf);
        }

        private void Create_Parameterizable(ILoadLinkExpression node)
        {
            var actual = _sut.Create(node);

            var asModelTree = actual.Projection(n => n.ModelType.Name);
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

        public class SubLoadLinkExpressionTreeLinkedSource : ILinkedSource<SubLoadLinkExpressionTreeContent> {
            public SubLoadLinkExpressionTreeContent Model { get; set; }
            public PersonLinkedSource Person { get; set; }
        }

        public class SubLoadLinkExpressionTreeContent {
            public string PersonId { get; set; }
        }
    }
}
