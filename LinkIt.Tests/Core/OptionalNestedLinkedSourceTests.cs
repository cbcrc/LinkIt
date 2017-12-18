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
    public class OptionalNestedLinkedSourceTests
    {
        private ILoadLinkProtocol _sut;

        public OptionalNestedLinkedSourceTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.MediaId,
                    linkedSource => linkedSource.Media
                );

            loadLinkProtocolBuilder.For<MediaLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_WithValue_ShouldLinkMediaAsync()
        {
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(
                new Model
                {
                    Id = "1",
                    MediaId = 32
                }
            );

            Assert.Equal(32, actual.Media.Model.Id);
            Assert.Equal("img32", actual.Media.SummaryImage.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_WithoutValue_ShouldLinkNullAsync()
        {
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(
                new Model
                {
                    Id = "1",
                    MediaId = null
                }
            );

            Assert.Null(actual.Media);
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public MediaLinkedSource Media { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public int? MediaId { get; set; }
        }
    }
}