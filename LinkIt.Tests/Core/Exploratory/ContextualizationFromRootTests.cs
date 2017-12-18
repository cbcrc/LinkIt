using System.Linq;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Exploratory
{
    public class ContextualizationFromRootTests
    {
        public ContextualizationFromRootTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PersonContextualizedLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource =>
                        linkedSource.Contextualization?.SummaryImageId ??
                        linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        private readonly ILoadLinkProtocol _sut;

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_WithContextualization_ShouldLinkOverriddenImageAsync()
        {
            var actual = await _sut.LoadLink<PersonContextualizedLinkedSource>().ByIdAsync(
                "32",
                linkedSource => linkedSource.Contextualization = new PersonContextualization
                {
                    Id = "32",
                    Name = null,
                    SummaryImageId = "overriden-image"
                }
            );

            Assert.Equal("overriden-image", actual.Contextualization?.SummaryImageId);
            Assert.Equal("overriden-image", actual.SummaryImage.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_WithContextualizationsAndSomeIdCannotBeResolved_ShouldOnlyContextextualizedResolvableIdsAsync()
        {
            var actual = await _sut.LoadLink<PersonContextualizedLinkedSource>().ByIdsAsync(
                new[] { "88", "cannot-be-resolved", "99" }.ToList(),
                (referenceIndex, linkedSource) => linkedSource.Contextualization = new PersonContextualization
                {
                    Id = "32",
                    Name = "Altered named",
                    SummaryImageId = "overriden-image-" + referenceIndex
                }
            );

            Assert.Equal(2, actual.Count);
            Assert.Equal("overriden-image-0", actual.First().Contextualization.SummaryImageId);
            Assert.Equal("overriden-image-0", actual.First().SummaryImage.Id);
            Assert.Equal("overriden-image-2", actual.Last().Contextualization.SummaryImageId);
            Assert.Equal("overriden-image-2", actual.Last().SummaryImage.Id);
        }
    }
}