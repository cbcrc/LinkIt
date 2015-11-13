using System;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using LinkIt.ConfigBuilders;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests
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
            var fakeReferenceLoader = new FakeReferenceLoader<Person, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            var actual = sut.LoadLink<PersonLinkedSource>().ById("dont-care");

            Assert.That(fakeReferenceLoader.IsDisposed, Is.True);
        }

        [Test]
        public void LoadLink_LinkedSourceWithoutLoadLinkExpressionAtRoot_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            var fakeReferenceLoader = new FakeReferenceLoader<Person, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            TestDelegate act = () => sut.LoadLink<PersonLinkedSource>().ById("dont-care");

            Assert.That(act, Throws.InvalidOperationException
                .With.Message.Contains("PersonLinkedSource").And
                .With.Message.Contains("root linked source")
            );

        }

    }
}