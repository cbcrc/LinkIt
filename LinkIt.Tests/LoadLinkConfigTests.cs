using System.Collections.Generic;
using ApprovalTests.Reporters;
using LinkIt.LinkTargets;
using LinkIt.LoadLinkExpressions;
using LinkIt.LoadLinkExpressions.Includes;
using LinkIt.LoadLinkExpressions.Includes.Interfaces;
using LinkIt.Protocols;
using NUnit.Framework;

namespace LinkIt.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkConfigTests
    {
        [Test]
        public void CreateLoadLinkConfig_ManyLoadLinkExpressionWithSameLinkTargetId_ShouldThrow()
        {
            var duplicate = new SingleValueLinkTarget<object, object>("the-duplicate-id", null);

            TestDelegate act = () => new LoadLinkConfig(
                new List<ILoadLinkExpression>{
                    new LoadLinkExpressionImpl<object, object, object, object>(
                        duplicate, null, 
                        new IncludeSet<object, object, object, object>(new Dictionary<object, IInclude>(),null)
                    ),
                    new LoadLinkExpressionImpl<object, object, object, object>(
                        duplicate, null,
                        new IncludeSet<object, object, object, object>(new Dictionary<object, IInclude>(),null)
                    )
                }
            );

            Assert.That(
                act, 
                Throws.ArgumentException
                    .With.Message.ContainsSubstring("link target id").And
                    .With.Message.ContainsSubstring("the-duplicate-id")
            );
        }

    }
}
