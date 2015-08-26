using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class NestedLinkedSourceTests
    {
        [Test]
        public void LoadLink_NestedLinkedSource()
        {
            var sut = TestHelper.CreateLoadLinkProtocol(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new LoadLinkExpression<PersonLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference
                    )
                },
                fixedValue: new NestedContent {
                    Id = 1,
                    PersonId = 32
                },
                getReferenceIdFunc: reference => reference.Id
            );

            var actual = sut.LoadLink<SingleReferenceLinkedSource, int, SingleReferenceContent>(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }


    public class NestedLinkedSource {
        public NestedContent Model { get; set; }
        public PersonLinkedSource Person { get; set; }
    }

    public class NestedContent {
        public int Id { get; set; }
        public int PersonId { get; set; }
    }

    public class PersonLinkedSource
    {
        public Person Model { get; set; }
        public Image SummaryImage{ get; set; }
    }
}
