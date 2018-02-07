// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Linq;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class NestedLinkedSourceTests
    {
        private ReferenceLoaderStub _referenceLoaderStub;
        private ILoadLinkProtocol _sut;

        public NestedLinkedSourceTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<NestedLinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.AuthorDetailId,
                    linkedSource => linkedSource.AuthorDetail)
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ClientSummaryId,
                    linkedSource => linkedSource.ClientSummary);

            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _referenceLoaderStub = new ReferenceLoaderStub();
            _sut = loadLinkProtocolBuilder.Build(() => _referenceLoaderStub);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedLinkedSourceAsync()
        {
            var actual = await _sut.LoadLink<NestedLinkedSource>().FromModelAsync(
                new NestedContent
                {
                    Id = 1,
                    AuthorDetailId = "32",
                    ClientSummaryId = "33"
                }
            );

            Assert.Equal("32", actual.AuthorDetail.Model.Id);
            Assert.Equal(actual.AuthorDetail.Model.SummaryImageId, actual.AuthorDetail.SummaryImage.Id);
            Assert.Equal("33", actual.ClientSummary.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_DifferendKindOfPersonInSameRootLinkedSource_ShouldNotLoadImageFromClientSummaryAsync()
        {
            await _sut.LoadLink<NestedLinkedSource>().FromModelAsync(
                new NestedContent
                {
                    Id = 1,
                    AuthorDetailId = "author-id",
                    ClientSummaryId = "client-summary-id"
                }
            );

            var imageLookupIds = _referenceLoaderStub.RecordedLookupIdContexts.Last().GetReferenceIds<Image, string>();

            Assert.Equal(new [] { "person-img-author-id" }, imageLookupIds);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedLinkedSourceWithoutReferenceId_ShouldLinkNullAsync()
        {
            var actual = await _sut.LoadLink<NestedLinkedSource>().FromModelAsync(
                new NestedContent
                {
                    Id = 1,
                    AuthorDetailId = null,
                    ClientSummaryId = "33"
                }
            );

            Assert.Null(actual.AuthorDetail);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedLinkedSourceCannotBeResolved_ShouldLinkNullAsync()
        {
            var actual = await _sut.LoadLink<NestedLinkedSource>().FromModelAsync(
                new NestedContent
                {
                    Id = 1,
                    AuthorDetailId = "cannot-be-resolved",
                    ClientSummaryId = "33"
                }
            );

            Assert.Null(actual.AuthorDetail);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedLinkedSourceRootCannotBeResolved_ShouldReturnNullAsRootAsync()
        {
            NestedContent model = null;

            var actual = await _sut.LoadLink<NestedLinkedSource>().FromModelAsync(model);

            Assert.Null(actual);
        }
    }



}