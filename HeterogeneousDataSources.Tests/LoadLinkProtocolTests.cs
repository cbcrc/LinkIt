using System;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
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
            loadLinkProtocolBuilder.For<WithoutReferenceLinkedSource>();
            var fakeReferenceLoader = new FakeReferenceLoader<SingleReferenceContent, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            var actual = sut.LoadLink<WithoutReferenceLinkedSource,string>("dont-care");

            Assert.That(fakeReferenceLoader.IsDisposed, Is.True);
        }

        [Test]
        public void LoadLink_ModelIdIsNull_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithoutReferenceLinkedSource>();
            var fakeReferenceLoader = new FakeReferenceLoader<SingleReferenceContent, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            TestDelegate act = () => sut.LoadLink<WithoutReferenceLinkedSource, string>(null);

            Assert.That(
                act, 
                Throws
                    .InstanceOf<ArgumentNullException>()
                    .With.Message.ContainsSubstring("modelId")
            );
        }
    }

    public class WithoutReferenceLinkedSource : ILinkedSource<Image> {
        public Image Model { get; set; }
    }
}