#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Linq;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;


namespace LinkIt.Tests.Core {
    public class NestedLinkedSourceTests
    {
        private ILoadLinkProtocol _sut;
        private ReferenceLoaderStub _referenceLoaderStub;

        [SetUp]
        public void SetUp() {
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
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "32",
                    ClientSummaryId = "33"
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Fact]
        public void LoadLink_DifferendKindOfPersonInSameRootLinkedSource_ShouldNotLoadImageFromClientSummary() {
            _sut.LoadLink<NestedLinkedSource>().FromModel(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "author-id",
                    ClientSummaryId = "client-summary-id"
                }
            );

            var imageLookupIds = _referenceLoaderStub.RecordedLookupIdContexts.Last().GetReferenceIds<Image, string>();

            ApprovalsExt.VerifyPublicProperties(imageLookupIds);
        }

        [Fact]
        public void LoadLink_NestedLinkedSourceWithoutReferenceId_ShouldLinkNull() {
            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = null,
                    ClientSummaryId = "33"
                }
            );

            Assert.That(actual.AuthorDetail, Is.Null);
        }

        [Fact]
        public void LoadLink_NestedLinkedSourceCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "cannot-be-resolved",
                    ClientSummaryId = "33"
                }
            );

            Assert.That(actual.AuthorDetail, Is.Null);
        }

        [Fact]
        public void LoadLink_NestedLinkedSourceRootCannotBeResolved_ShouldReturnNullAsRoot() {
            NestedContent model = null;

            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(model);

            Assert.That(actual, Is.Null);
        }
    }


    public class NestedLinkedSource:ILinkedSource<NestedContent>
    {
        public NestedContent Model { get; set; }
        public PersonLinkedSource AuthorDetail { get; set; }
        public Person ClientSummary { get; set; }
    }

    public class NestedContent {
        public int Id { get; set; }
        public string AuthorDetailId { get; set; }
        public string ClientSummaryId { get; set; }
    }

    public class PersonLinkedSource: ILinkedSource<Person>
    {
        public Person Model { get; set; }
        public Image SummaryImage{ get; set; }
    }
}
