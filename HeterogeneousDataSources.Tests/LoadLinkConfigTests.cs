using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkConfigTests
    {
        [Test]
        public void CreateLoadLinkConfig_ManyRootExpressionForTheSameChildLinkedSource_ShouldThrow()
        {
            TestDelegate act = () => new LoadLinkConfig(
                new List<ILoadLinkExpression>
                {
                    new RootLoadLinkExpression<PersonLinkedSource, Person, string>(),
                    new RootLoadLinkExpression<PersonLinkedSource, Person, int>(),
                }
            );

            Assert.That(act, Throws.ArgumentException.With.Message.ContainsSubstring("PersonLinkedSource"));
        }
    }
}
