#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LoadLinkExpressionOverridingTests
    {
        public LoadLinkExpressionOverridingTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource =>
                    {
                        throw new Exception("Not overridden!");
#pragma warning disable 162
                        return linkedSource.Model.SummaryImageId;
#pragma warning restore 162
                    },
                    linkedSource => linkedSource.SummaryImage
                );

                //override
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId + "-overridden",
                    linkedSource => linkedSource.SummaryImage
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        private readonly ILoadLinkProtocol _sut;

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_WithOverriddenLoadLinkExpression_ShouldUseOverriddenLoadLinkExpressionAsync()
        {
            var actual = await _sut.LoadLink<SingleReferenceLinkedSource>().FromModelAsync(
                new SingleReferenceContent
                {
                    Id = "1",
                    SummaryImageId = "a"
                }
            );

            Assert.Equal("a-overridden", actual.SummaryImage.Id);
        }
    }
}