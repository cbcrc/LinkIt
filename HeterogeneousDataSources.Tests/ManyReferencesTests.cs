using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ManyReferencesTests
    {
        [Test]
        public void LoadLink_ManyReferences()
        {
            var loadLinkExpressions = new List<ILoadLinkExpression>{
                new LoadLinkExpression<ManyReferencesLinkedSource,Image, string>(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    (linkedSource, reference) => linkedSource.SummaryImage = reference
                ),
                new LoadLinkExpression<ManyReferencesLinkedSource,Image, string>(
                    linkedSource => linkedSource.Model.AuthorImageId,
                    (linkedSource, reference) => linkedSource.AuthorImage = reference
                ),
                new LoadLinkExpressionForList<ManyReferencesLinkedSource,Image, string>(
                    linkedSource => linkedSource.Model.FavoriteImageIds,
                    (linkedSource, reference) => linkedSource.FavoriteImages = reference
                )
            };
            var sut = new LoadLinkProtocol(
                TestUtil.ReferenceTypeConfigs,
                loadLinkExpressions
            );
            var content = new ManyReferencesContent {
                Id = 1,
                SummaryImageId = "summary-image-id",
                AuthorImageId = "author-image-id",
                FavoriteImageIds = new List<string>{"one","two"}
            };
            var contentLinkedSource = new ManyReferencesLinkedSource{Model=content};

            sut.LoadLink(contentLinkedSource);
            
            ApprovalsExt.VerifyPublicProperties(contentLinkedSource);
        }
    }

    public class ManyReferencesLinkedSource {
        public ManyReferencesContent Model { get; set; }
        public Image SummaryImage { get; set; }
        public Image AuthorImage { get; set; }
        public List<Image> FavoriteImages { get; set; }
    }

    public class ManyReferencesContent {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
        public string AuthorImageId { get; set; }
        public List<string> FavoriteImageIds { get; set; }
    }

}