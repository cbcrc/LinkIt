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
            loadLinkProtocolBuilder.For<WithoutReferenceLinkedSource>()
                .IsRoot<string>();
            var fakeReferenceLoader = new FakeReferenceLoader<SingleReferenceContent, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            var actual = sut.LoadLink<WithoutReferenceLinkedSource>("dont-care");

            Assert.That(fakeReferenceLoader.IsDisposed, Is.True);
        }

        [Test]
        public void LoadLink_ModelIdIsNull_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithoutReferenceLinkedSource>();
            var fakeReferenceLoader = new FakeReferenceLoader<SingleReferenceContent, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            TestDelegate act = () => sut.LoadLink<WithoutReferenceLinkedSource>(null);

            Assert.That(
                act, 
                Throws
                    .InstanceOf<ArgumentNullException>()
                    .With.Message.ContainsSubstring("modelId")
            );
        }

        [Test]
        public void LoadLink_NotARootLinkedSourceType_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithoutReferenceLinkedSource>();
            var fakeReferenceLoader = new FakeReferenceLoader<SingleReferenceContent, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            TestDelegate act = () => sut.LoadLink<WithoutReferenceLinkedSource>("dont-care");

            Assert.That(
                act,
                Throws.ArgumentException.With.Message.ContainsSubstring("WithoutReferenceLinkedSource")
            );
        }
    }

    public class WithoutReferenceLinkedSource : ILinkedSource<Image> {
        public Image Model { get; set; }
    }
}