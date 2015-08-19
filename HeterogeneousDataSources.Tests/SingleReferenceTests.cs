using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
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
            var loadLinkExpressions = new List<LoadLinkExpression<SingleReferenceLinkedSource, Image, string>>{
                    new LoadLinkExpression<SingleReferenceLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference
                    )
                };

            var content = new SingleReferenceContent{
                Id = 1,
                SummaryImageId = "a"
            };
            var contentLinkedSource = new SingleReferenceLinkedSource(content);

            var asLoadExpressions = loadLinkExpressions
                .Cast<ILoadExpression<SingleReferenceLinkedSource,string>>()
                .ToList();

            var loader = new Loader();
            var dataContext = loader.Load(contentLinkedSource, asLoadExpressions, TestUtil.ImageReferenceTypeConfig);

            var contentLinkedSourceLinkExpressions = loadLinkExpressions
                .Cast<ILinkExpression<SingleReferenceLinkedSource>>()
                .ToList();

            var linker = new Linker();
            linker.Link(dataContext, contentLinkedSource, contentLinkedSourceLinkExpressions);
            ApprovalsExt.VerifyPublicProperties(contentLinkedSource);
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
