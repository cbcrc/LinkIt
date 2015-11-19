using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Tests.Shared;
using NUnit.Framework;

namespace LinkIt.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkProtocolTests
    {
        [Test]
        public void LoadLink_ShouldDisposeLoader()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            var sut = new ReferenceLoaderStub();
            var loadLinkConfig = loadLinkProtocolBuilder.Build(()=>sut);

            loadLinkConfig.LoadLink<PersonLinkedSource>().ById("dont-care");

            Assert.That(sut.IsDisposed, Is.True);
        }

        [Test]
        public void LoadLink_LinkedSourceWithoutLoadLinkExpressionAtRoot_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            var sut = loadLinkProtocolBuilder.Build(()=>new ReferenceLoaderStub());

            TestDelegate act = () => sut.LoadLink<PersonLinkedSource>().ById("dont-care");

            Assert.That(act, Throws.InvalidOperationException
                .With.Message.Contains("PersonLinkedSource").And
                .With.Message.Contains("root linked source")
            );

        }

    }
}