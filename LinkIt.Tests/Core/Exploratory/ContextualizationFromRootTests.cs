using System.Linq;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;

namespace LinkIt.Tests.Core.Exploratory {
    public class ContextualizationFromRootTests{
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PersonContextualizedLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => 
                        linkedSource.Contextualization?.SummaryImageId ?? 
                        linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public void LoadLink_WithContextualization_ShouldLinkOverriddenImage() {
            var actual = _sut.LoadLink<PersonContextualizedLinkedSource>().ById(
                "32",
                linkedSource => linkedSource.Contextualization = new PersonContextualization {
                    Id = "32",
                    Name = null,
                    SummaryImageId = "overriden-image"
                }
            );

            Assert.That(actual.Contextualization?.SummaryImageId, Is.EqualTo("overriden-image"));
            Assert.That(actual.SummaryImage.Id, Is.EqualTo("overriden-image"));
        }

        [Fact]
        public void LoadLink_WithContextualizationsAndSomeIdCannotBeResolved_ShouldOnlyContextextualizedResolvableIds() {
            var actual = _sut.LoadLink<PersonContextualizedLinkedSource>().ByIds(
                new [] {"88", "cannot-be-resolved", "99", }.ToList(),
                (referenceIndex,linkedSource) => linkedSource.Contextualization = new PersonContextualization {
                    Id = "32",
                    Name = "Altered named",
                    SummaryImageId = "overriden-image-" + referenceIndex
                }
            );

            Assert.That(actual.Count, Is.EqualTo(2));
            Assert.That(actual.First().Contextualization?.SummaryImageId, Is.EqualTo("overriden-image-0"));
            Assert.That(actual.First().SummaryImage.Id, Is.EqualTo("overriden-image-0"));
            Assert.That(actual.Last().Contextualization?.SummaryImageId, Is.EqualTo("overriden-image-2"));
            Assert.That(actual.Last().SummaryImage.Id, Is.EqualTo("overriden-image-2"));

        }
    }

    
}
