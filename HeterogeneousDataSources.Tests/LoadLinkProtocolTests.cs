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
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );
            var fakeReferenceLoader = new FakeReferenceLoader<Person, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            var actual = sut.LoadLink<PersonLinkedSource>().ById("dont-care");

            Assert.That(fakeReferenceLoader.IsDisposed, Is.True);
        }
    }
}