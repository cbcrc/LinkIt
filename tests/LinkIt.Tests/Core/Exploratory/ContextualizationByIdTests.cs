// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Threading.Tasks;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Exploratory
{
    public class ContextualizationByIdTests
    {
        public ContextualizationByIdTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithContextualizedReferenceLinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.PersonContextualization.Id,
                    linkedSource => linkedSource.Person,
                    (linkedSource, childLinkedSource) =>
                        childLinkedSource.Contextualization = linkedSource.Model.PersonContextualization
                );
            loadLinkProtocolBuilder.For<PersonContextualizedLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource =>
                        linkedSource.Contextualization.SummaryImageId ??
                        linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        private readonly ILoadLinkProtocol _sut;

        [Fact]
        public async Task LoadLink_WithContextualization_ShouldLinkOverriddenImage()
        {
            var model = new WithContextualizedReference
            {
                Id = "1",
                PersonContextualization = new PersonContextualization
                {
                    Id = "32",
                    Name = "Altered named",
                    SummaryImageId = "overriden-image"
                }
            };
            var actual = await _sut.LoadLink<WithContextualizedReferenceLinkedSource>().FromModelAsync(model);

            Assert.Same(model.PersonContextualization, actual.Person.Contextualization);
            Assert.Equal(model.PersonContextualization.Id, actual.Person.Model.Id);
            Assert.Same(model.PersonContextualization.SummaryImageId, actual.Person.SummaryImage.Id);
        }

        [Fact]
        public async Task LoadLink_WithoutContextualization_ShouldLinkDefaultImage()
        {
            var model = new WithContextualizedReference
            {
                Id = "1",
                PersonContextualization = new PersonContextualization
                {
                    Id = "32",
                    Name = "Altered named",
                    SummaryImageId = null
                }
            };
            var actual = await _sut.LoadLink<WithContextualizedReferenceLinkedSource>().FromModelAsync(model);

            Assert.Same(model.PersonContextualization, actual.Person.Contextualization);
            Assert.Equal($"person-img-{model.PersonContextualization.Id}", actual.Person.SummaryImage.Id);
        }
    }
}