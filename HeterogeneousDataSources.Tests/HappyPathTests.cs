using System.Collections.Generic;
using ApprovalTests.Reporters;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class HappyPathTests
    {
        [Test]
        public void DoIt()
        {
            var loadLinkExpressions = new List<ILoadLinkExpression>{
                    new LoadLinkExpression<ContentLinkedSource,Image>(
                        (linkedSource) => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference,
                        (image) => image.Id
                    )
                };

            var content = new Content{
                Id = 1,
                SummaryImageId = "a"
            };
            var contentLinkedSource = new ContentLinkedSource(content);

            var loader = new Loader();
            var dataContext = loader.Load(contentLinkedSource, loadLinkExpressions);
            var linker = new Linker();
            linker.Link(dataContext, contentLinkedSource, loadLinkExpressions);
            ApprovalsExt.VerifyPublicProperties(contentLinkedSource);
        }
    }


    public class ContentLinkedSource {
        //stle: two steps loading sucks!
        public ContentLinkedSource(Content model)
        {
            Model = model;
        }

        public Content Model { get; private set; }
        public Image SummaryImage;
    }

    public class Content {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
    }

    public class Image {
        public string Id { get; set; }
        public string Alt { get; set; }
    }


}
