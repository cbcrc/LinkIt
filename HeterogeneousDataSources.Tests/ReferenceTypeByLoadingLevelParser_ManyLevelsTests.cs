using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTypeByLoadingLevelParser_ManyLevelsTests
    {
        [Test]
        public void ParseReferenceTypeByLoadingLevel_ManyLevels()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<ManyLoadingLevelContentLinkedSource>()
                .IsRoot<string>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.BlogPostId,
                    linkedSource => linkedSource.BlogPost
                )
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
            var rootLoadLinkExpression = loadLinkProtocolBuilder.GetLoadLinkExpressions()[0];
            var sut = TestSetupHelper.CreateReferenceTypeByLoadingLevelParser(loadLinkProtocolBuilder);

            var actual = sut.ParseReferenceTypeByLoadingLevel(rootLoadLinkExpression);

            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }

    public class ManyLoadingLevelContentLinkedSource : ILinkedSource<ManyLoadingLevelContent> {
        public ManyLoadingLevelContent Model { get; set; }
        public BlogPost BlogPost { get; set; }
        public Image PreImage { get; set; }
        public PersonLinkedSource Person { get; set; }
        public Image PostImage { get; set; }
    }

    public class ManyLoadingLevelContent {
        public string Id { get; set; }
        public string BlogPostId { get; set; }
        public string PreImageId { get; set; }
        public string PersonId { get; set; }
        public string PostImageId { get; set; }
    }

    public class BlogPost {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}
