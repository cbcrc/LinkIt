using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests {
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
                        duplicate, null, null, new Dictionary<object, IInclude>()
                    ),
                    new LoadLinkExpressionImpl<object, object, object, object>(
                        duplicate, null, null, new Dictionary<object, IInclude>()
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
