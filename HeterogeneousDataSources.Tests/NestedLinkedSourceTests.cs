using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
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
            var loadLinkExpressions = new List<ILoadLinkExpression>{
                };

            var sut = new LoadLinkProtocol(
                TestUtil.ReferenceTypeConfigs,
                loadLinkExpressions
            );
            var content = new NestedContent{
                Id = 1,
                PersonId = "john"
            };
            var contentLinkedSource = new NestedLinkedSource{ Model = content };

            sut.LoadLink(contentLinkedSource);

            ApprovalsExt.VerifyPublicProperties(contentLinkedSource);
        }
    }


    public class NestedLinkedSource {
        public NestedContent Model { get; set; }
        public PersonLinkedSource Person { get; set; }
    }

    public class NestedContent {
        public int Id { get; set; }
        public string PersonId { get; set; }
    }

    public class PersonLinkedSource
    {
        public Person Model { get; set; }
        public Image SummaryImage{ get; set; }
    }
}
