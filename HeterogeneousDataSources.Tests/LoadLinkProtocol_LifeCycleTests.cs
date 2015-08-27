using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkProtocol_LifeCycleTests
    {
        [Test]
        public void LoadLink_ShouldDisposeLoader()
        {
            var referenceLoader = new FakeReferenceLoader();
            var sut = new LoadLinkProtocol(referenceLoader, new List<ILoadLinkExpression>());
            
            var actual = sut.LoadLink<WithoutReferenceLinkedSource, string, Image>("dont-care");

            Assert.That(referenceLoader.IsDisposed, Is.True);
        }
    }

    public class WithoutReferenceLinkedSource : ILinkedSource<Image> {
        public Image Model { get; set; }
    }
}