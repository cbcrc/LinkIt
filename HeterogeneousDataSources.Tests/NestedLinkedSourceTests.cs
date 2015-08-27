using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
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
                    new NestedLinkedSourceLoadLinkExpression<NestedLinkedSource, PersonLinkedSource, Person, int>(
                        linkedSource => linkedSource.Model.PersonId,
                        (linkedSource, reference) => linkedSource.Person = reference
                    ),
                    new ReferenceLoadLinkExpression<PersonLinkedSource,Image, string>(
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

            var actual = sut.LoadLink<NestedLinkedSource, int, NestedContent>(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }


    public class NestedLinkedSource:ILinkedSource<NestedContent>
    {
        public NestedContent Model { get; set; }
        public PersonLinkedSource Person { get; set; }
    }

    public class NestedContent {
        public int Id { get; set; }
        public int PersonId { get; set; }
    }

    public class PersonLinkedSource: ILinkedSource<Person>
    {
        public Person Model { get; set; }
        public Image SummaryImage{ get; set; }
    }
}
