// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using LinkIt.ConfigBuilders;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LoadLinkProtocolTests
    {
        [Fact]
        public async Task LoadLink_ShouldDisposeLoader()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);
            var sut = new ReferenceLoaderStub();
            var loadLinkConfig = loadLinkProtocolBuilder.Build(() => sut);

            await loadLinkConfig.LoadLink<PersonLinkedSource>().ByIdAsync("dont-care");

            Assert.True(sut.IsDisposed);
        }

        [Fact]
        public async Task LoadLink_LinkedSourceWithoutLoadLinkExpressionAtRoot_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            var sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            Func<Task> act = async () => await sut.LoadLink<PersonLinkedSource>().ByIdAsync("dont-care");

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
            Assert.Contains("PersonLinkedSource", ex.Message);
            Assert.Contains("root linked source", ex.Message);
        }
    }
}