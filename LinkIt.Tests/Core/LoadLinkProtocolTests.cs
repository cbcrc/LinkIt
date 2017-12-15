#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using LinkIt.ConfigBuilders;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LoadLinkProtocolTests
    {
        [Fact]
        public void LoadLink_ShouldDisposeLoader()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);
            var sut = new ReferenceLoaderStub();
            var loadLinkConfig = loadLinkProtocolBuilder.Build(() => sut);

            loadLinkConfig.LoadLink<PersonLinkedSource>().ById("dont-care");

            Assert.True(sut.IsDisposed);
        }

        [Fact]
        public void LoadLink_LinkedSourceWithoutLoadLinkExpressionAtRoot_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            var sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            Action act = () => sut.LoadLink<PersonLinkedSource>().ById("dont-care");

            var ex = Assert.Throws<InvalidOperationException>(act);
            Assert.Contains("PersonLinkedSource", ex.Message);
            Assert.Contains("root linked source", ex.Message);
        }
    }
}