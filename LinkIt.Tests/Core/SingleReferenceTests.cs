#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class SingleReferenceTests
    {
        private ILoadLinkProtocol _sut;

        public SingleReferenceTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public void LoadLink_SingleReference()
        {
            var actual = _sut.LoadLink<SingleReferenceLinkedSource>().FromModel(
                new SingleReferenceContent
                {
                    Id = "1",
                    SummaryImageId = "a"
                }
            );

            Assert.Equal("a", actual.SummaryImage.Id);
        }

        [Fact]
        public void LoadLink_SingleReferenceWithoutReferenceId_ShouldLinkNull()
        {
            var actual = _sut.LoadLink<SingleReferenceLinkedSource>().FromModel(
                new SingleReferenceContent
                {
                    Id = "1",
                    SummaryImageId = null
                }
            );

            Assert.Null(actual.SummaryImage);
        }

        [Fact]
        public void LoadLink_SingleReferenceCannotBeResolved_ShouldLinkNull()
        {
            var actual = _sut.LoadLink<SingleReferenceLinkedSource>().FromModel(
                new SingleReferenceContent
                {
                    Id = "1",
                    SummaryImageId = "cannot-be-resolved"
                }
            );

            Assert.Null(actual.SummaryImage);
        }
    }

    public class SingleReferenceLinkedSource : ILinkedSource<SingleReferenceContent>
    {
        public Image SummaryImage { get; set; }
        public SingleReferenceContent Model { get; set; }
    }

    public class SingleReferenceContent
    {
        public string Id { get; set; }
        public string SummaryImageId { get; set; }
    }
}