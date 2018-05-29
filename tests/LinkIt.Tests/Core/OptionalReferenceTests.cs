// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class OptionalReferenceTests
    {
        private readonly ILoadLinkProtocol _sut;

        public OptionalReferenceTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.MediaId,
                    linkedSource => linkedSource.Media
                );

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

            Assert.Equal(32, actual.Media.Id);
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
            public Media Media { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public int? MediaId { get; set; }
        }
    }
}