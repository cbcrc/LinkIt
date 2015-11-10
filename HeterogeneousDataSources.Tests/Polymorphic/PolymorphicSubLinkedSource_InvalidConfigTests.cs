using ApprovalTests.Reporters;
using HeterogeneousDataSources.ConfigBuilders;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicSubLinkedSource_InvalidConfigTests {
        [Test]
        public void LoadLink_PolymorphicSubLinkedSourceWithWrongLinkedSourceModelType_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            TestDelegate act = () => loadLinkProtocolBuilder.For<PolymorphicSubLinkedSourceTests.LinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes
                        .Include<PolymorphicSubLinkedSourceTests.WebPageReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                            "web-page",
                            link => "a string is not a WebPageReference"
                        )
                );

            Assert.That(
                act,
                Throws.ArgumentException
                    .With.Message.Contains("LinkedSource/Target").And
                    .With.Message.Contains("WebPageReference").And
                    .With.Message.Contains("getSubLinkedSourceModel")
            );
        }
    }
}
