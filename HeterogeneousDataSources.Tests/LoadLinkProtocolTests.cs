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

            var actual = sut.LoadLink<WithoutReferenceLinkedSource>().ById("dont-care");

            Assert.That(fakeReferenceLoader.IsDisposed, Is.True);
        }

        [Test]
        public void LoadLink_ModelIdIsNull_ShouldLinkNull() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithoutReferenceLinkedSource>();
            var fakeReferenceLoader = new FakeReferenceLoader<SingleReferenceContent, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            var actual = sut.LoadLink<WithoutReferenceLinkedSource>().ById<string>(null);

            Assert.That(actual, Is.Null);
        }
    }

    public class WithoutReferenceLinkedSource : ILinkedSource<Image> {
        public Image Model { get; set; }
    }
}