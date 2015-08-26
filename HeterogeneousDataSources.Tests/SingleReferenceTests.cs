using System;
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
            var sut = TestHelper.CreateLoadLinkProtocol(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                                new LoadLinkExpression<SingleReferenceLinkedSource, Image, string>(
                                    linkedSource => linkedSource.Model.SummaryImageId,
                                    (linkedSource, reference) => linkedSource.SummaryImage = reference
                                )
                            },
                fixedValue: new SingleReferenceContent {
                    Id = 1,
                    SummaryImageId = "a"
                },
                getReferenceIdFunc: reference => reference.Id
            );

            var actual = sut.LoadLink<SingleReferenceLinkedSource, int, SingleReferenceContent>(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }

    public class SingleReferenceLinkedSource: ILinkedSource<SingleReferenceContent>
    {
        //stle: two steps loading sucks!
        public SingleReferenceContent Model { get; set; }
        public Image SummaryImage{ get; set; }
    }

    public class SingleReferenceContent {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
    }
}
