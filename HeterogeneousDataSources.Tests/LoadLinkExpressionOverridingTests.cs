using System;
using System.Diagnostics;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.ConfigBuilders;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkExpressionOverridingTests
    {
        private FakeReferenceLoader<SingleReferenceContent, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReference(
                    linkedSource =>
                    {
                        throw new Exception("Not overridden!");
                        return linkedSource.Model.SummaryImageId;
                    },
                    linkedSource => linkedSource.SummaryImage
                );

            //override
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId+"-overridden",
                    linkedSource => linkedSource.SummaryImage
                );

            _fakeReferenceLoader = 
                new FakeReferenceLoader<SingleReferenceContent, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_WithOverriddenLoadLinkExpression_ShouldUseOverriddenLoadLinkExpression()
        {
            _fakeReferenceLoader.FixValue(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "a"
                }
                );

            var actual = _sut.LoadLink<SingleReferenceLinkedSource>().ById("1");

            Assert.That(actual.SummaryImage.Id, Is.EqualTo("a-overridden"));
        }

    }
}