using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.ConfigBuilders;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.ReferenceTrees;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTree_SubLinkedSourceTests
    {
        private LoadLinkConfig _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.PostThread,
                    linkedSource => linkedSource.PostThread
                );
            loadLinkProtocolBuilder.For<PostThreadLinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.Posts,
                    linkedSource => linkedSource.Posts
                )
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.AuthorId,
                    linkedSource => linkedSource.Author
                );
            loadLinkProtocolBuilder.For<PostLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            _sut = new LoadLinkConfig(loadLinkProtocolBuilder.GetLoadLinkExpressions());
        }

        [Test]
        public void CreateRootReferenceTree(){
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
            public PostThreadLinkedSource PostThread { get; set; }
        }

        public class PostThreadLinkedSource : ILinkedSource<PostThread> {
            public PostThread Model { get; set; }
            public List<PostLinkedSource> Posts { get; set; }
            public Person Author { get; set; }
        }

        public class PostLinkedSource : ILinkedSource<Post> {
            public Post Model { get; set; }
            public Image SummaryImage { get; set; }
        }

        public class Model {
            public int Id { get; set; }
            public PostThread PostThread { get; set; }
        }

        public class PostThread{
            public string AuthorId { get; set; }
            public List<Post> Posts { get; set; }
        }

        public class Post{
            public string SummaryImageId { get; set; }
        }
    }
}
