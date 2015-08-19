using System.Collections.Generic;
using ApprovalTests.Reporters;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class MultipleReferencesTypeTests
    {
        [Test]
        public void LoadLink_MultipleReferencesTypeTests()
        {
            var loadLinkExpressions = new List<ILoadLinkExpression>{
                    new LoadLinkExpression<MultipleReferencesTypeLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference
                    ),
                    new LoadLinkExpression<MultipleReferencesTypeLinkedSource,Person, int>(
                        linkedSource => linkedSource.Model.AuthorId,
                        (linkedSource, reference) => linkedSource.Author = reference
                    )
                };
            var sut = new LoadLinkProtocol(
                TestUtil.ReferenceTypeConfigs,
                loadLinkExpressions
            );
            var content = new MultipleReferencesTypeContent() {
                Id = 1,
                SummaryImageId = "a",
                AuthorId = 32
            };
            var contentLinkedSource = new MultipleReferencesTypeLinkedSource(content);

            sut.LoadLink(contentLinkedSource);

            ApprovalsExt.VerifyPublicProperties(contentLinkedSource);
        }
    }


    public class MultipleReferencesTypeLinkedSource {
        public MultipleReferencesTypeLinkedSource(MultipleReferencesTypeContent model)
        {
            Model = model;
        }

        public MultipleReferencesTypeContent Model { get; private set; }
        public Image SummaryImage;
        public Person Author;
    }

    public class MultipleReferencesTypeContent {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
        public int AuthorId { get; set; }
    }
}
