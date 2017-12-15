#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

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
        public void LoadLink_NestedLinkedSource()
        {
            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(
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
        public void LoadLink_DifferendKindOfPersonInSameRootLinkedSource_ShouldNotLoadImageFromClientSummary()
        {
            _sut.LoadLink<NestedLinkedSource>().FromModel(
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
        public void LoadLink_NestedLinkedSourceWithoutReferenceId_ShouldLinkNull()
        {
            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(
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
        public void LoadLink_NestedLinkedSourceCannotBeResolved_ShouldLinkNull()
        {
            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(
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
        public void LoadLink_NestedLinkedSourceRootCannotBeResolved_ShouldReturnNullAsRoot()
        {
            NestedContent model = null;

            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(model);

            Assert.Null(actual);
        }
    }



}