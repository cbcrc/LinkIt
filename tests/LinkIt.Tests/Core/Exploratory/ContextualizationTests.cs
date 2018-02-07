// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Exploratory
{
    public class ContextualizationTests
    {
        public ContextualizationTests()
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
        public async System.Threading.Tasks.Task LoadLink_WithContextualization_ShouldLinkOverriddenImageAsync()
        {
            var actual = await _sut.LoadLink<WithContextualizedReferenceLinkedSource>().FromModelAsync(
                new WithContextualizedReference
                {
                    Id = "1",
                    PersonContextualization = new PersonContextualization
                    {
                        Id = "32",
                        Name = "Altered named",
                        SummaryImageId = "overriden-image"
                    }
                }
            );

            Assert.Equal("overriden-image", actual.Person.Contextualization.SummaryImageId);
            Assert.Equal("overriden-image", actual.Person.SummaryImage.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_WithoutContextualization_ShouldLinkDefaultImageAsync()
        {
            var actual = await _sut.LoadLink<WithContextualizedReferenceLinkedSource>().FromModelAsync(
                new WithContextualizedReference
                {
                    Id = "1",
                    PersonContextualization = new PersonContextualization
                    {
                        Id = "32",
                        Name = "Altered named",
                        SummaryImageId = null
                    }
                }
            );

            Assert.Null(actual.Person.Contextualization.SummaryImageId);
            Assert.Equal("person-img-32", actual.Person.SummaryImage.Id);
        }
    }
}