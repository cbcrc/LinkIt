#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;

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
                    linkedSource => linkedSource.SummaryImage
                );
            var sut = new ReferenceLoaderStub();
            var loadLinkConfig = loadLinkProtocolBuilder.Build(()=>sut);

            loadLinkConfig.LoadLink<PersonLinkedSource>().ById("dont-care");

            Assert.That(sut.IsDisposed, Is.True);
        }

        [Fact]
        public void LoadLink_LinkedSourceWithoutLoadLinkExpressionAtRoot_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            var sut = loadLinkProtocolBuilder.Build(()=>new ReferenceLoaderStub());

            TestDelegate act = () => sut.LoadLink<PersonLinkedSource>().ById("dont-care");

            Assert.That(act, Throws.InvalidOperationException
                .With.Message.Contains("PersonLinkedSource").And
                .With.Message.Contains("root linked source")
            );

        }

    }
}