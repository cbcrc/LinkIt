#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;

namespace LinkIt.Tests.Core
{
    public class LoadLinkExpressionOverridingTests
    {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource =>
                    {
                        throw new Exception("Not overridden!");
                        return linkedSource.Model.SummaryImageId;
                    },
                    linkedSource => linkedSource.SummaryImage
                );

            //override
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId+"-overridden",
                    linkedSource => linkedSource.SummaryImage
                );

            _sut = loadLinkProtocolBuilder.Build(()=>new ReferenceLoaderStub());
        }

        [Fact]
        public void LoadLink_WithOverriddenLoadLinkExpression_ShouldUseOverriddenLoadLinkExpression()
        {
            var actual = _sut.LoadLink<SingleReferenceLinkedSource>().FromModel(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "a"
                }
            );

            Assert.That(actual.SummaryImage.Id, Is.EqualTo("a-overridden"));
        }

    }
}