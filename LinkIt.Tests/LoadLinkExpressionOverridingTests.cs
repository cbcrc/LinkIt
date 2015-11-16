using System;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Protocols;
using LinkIt.Tests.Shared;
using NUnit.Framework;

namespace LinkIt.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkExpressionOverridingTests
    {
        private LoadLinkConfig _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource =>
                    {
                        throw new Exception("Not overridden!");
                        return linkedSource.Model.SummaryImageId;
                    },
                    linkedSource => linkedSource.SummaryImage
                );

            //override
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId+"-overridden",
                    linkedSource => linkedSource.SummaryImage
                );

            _sut = loadLinkProtocolBuilder.Build(()=>new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_WithOverriddenLoadLinkExpression_ShouldUseOverriddenLoadLinkExpression()
        {
            var actual = _sut.LoadLink<SingleReferenceLinkedSource>().FromModel(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "a"
                }
            );

            Assert.That(actual.SummaryImage.Id, Is.EqualTo("a-overridden"));
        }

    }
}