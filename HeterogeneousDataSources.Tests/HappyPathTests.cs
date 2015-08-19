using System.Collections.Generic;
using System.Linq;
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
            var loadLinkExpressions = new List<LoadLinkExpression<ContentLinkedSource, Image, string>>{
                    new LoadLinkExpression<ContentLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference
                    )
                };

            var futureReferenceLoader = new ReferenceTypeConfig<Image,string>(
                image => image.Id,
                ids => new ImageRepository().GetByIds(ids)
            );

            var content = new Content{
                Id = 1,
                SummaryImageId = "a"
            };
            var contentLinkedSource = new ContentLinkedSource(content);

            var asLoadExpressions = loadLinkExpressions
                .Cast<ILoadExpression<ContentLinkedSource,string>>()
                .ToList();

            var loader = new Loader();
            var dataContext = loader.Load(contentLinkedSource, asLoadExpressions, futureReferenceLoader);

            var contentLinkedSourceLinkExpressions = loadLinkExpressions
                .Cast<ILinkExpression<ContentLinkedSource>>()
                .ToList();

            var linker = new Linker();
            linker.Link(dataContext, contentLinkedSource, contentLinkedSourceLinkExpressions);
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
