using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class SingleReferenceTests
    {
        [Test]
        public void LoadLink_SingleReference()
        {
            var loadLinkExpressions = new List<ILoadLinkExpression>{
                    new LoadLinkExpression<SingleReferenceLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference
                    )
                };
            var sut = new LoadLinkProtocol(
                new FakeReferenceLoader(), 
                loadLinkExpressions
            );
            var content = new SingleReferenceContent{
                Id = 1,
                SummaryImageId = "a"
            };
            var contentLinkedSource = new SingleReferenceLinkedSource(content);

            sut.LoadLink(contentLinkedSource);

            ApprovalsExt.VerifyPublicProperties(contentLinkedSource);
        }

        [Test]
        public void LoadLink_ShouldDisposeLoader() {
            var loadLinkExpressions = new List<ILoadLinkExpression>();

            var referenceLoader = new FakeReferenceLoader();
            var sut = new LoadLinkProtocol(
                referenceLoader,
                loadLinkExpressions
            );
            var content = new SingleReferenceContent {
                Id = 1,
                SummaryImageId = "a"
            };
            var contentLinkedSource = new SingleReferenceLinkedSource(content);

            sut.LoadLink(contentLinkedSource);

            Assert.That(referenceLoader.IsDisposed, Is.True);
        }
    }


    public class SingleReferenceLinkedSource {
        //stle: two steps loading sucks!
        public SingleReferenceLinkedSource(SingleReferenceContent model)
        {
            Model = model;
        }

        public SingleReferenceContent Model { get; private set; }
        public Image SummaryImage;
    }

    public class SingleReferenceContent {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
    }
}
