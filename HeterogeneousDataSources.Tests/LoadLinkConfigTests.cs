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
        public void CreateLoadLinkConfig_ManyRootExpressionForTheSameChildLinkedSource_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .IsRoot<string>()
                .IsRoot<int>();

            TestDelegate act = () => new LoadLinkConfig(loadLinkProtocolBuilder.GetLoadLinkExpressions());

            Assert.That(act, Throws.ArgumentException.With.Message.ContainsSubstring("PersonLinkedSource"));
        }
    }
}
