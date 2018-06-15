// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using LinkIt.ConfigBuilders;
using LinkIt.Shared;
using Xunit;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PolymorphicSubLinkedSource_InvalidConfigTests
    {
        [Fact]
        public void LoadLink_PolymorphicSubLinkedSourceWithWrongLinkedSourceModelType_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            Action act = () => loadLinkProtocolBuilder.For<PolymorphicSubLinkedSourceTests.LinkedSource>()
                .LoadLinkPolymorphic(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes.Include<PolymorphicSubLinkedSourceTests.WebPageReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                        "web-page",
                        _ => "a string is not a WebPageReference"
                    )
                );

            var ex = Assert.Throws<LinkItException>(act);
            Assert.Contains("Invalid configuration", ex.Message);
            Assert.Contains("linkedSource.Target", ex.Message);
            Assert.Contains("WebPageReference", ex.Message);
            Assert.Contains("getNestedLinkedSourceModel", ex.Message);
        }

        [Fact]
        public void LoadLink_WithDiscriminantDuplicate_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            Action act = () => loadLinkProtocolBuilder.For<PolymorphicSubLinkedSourceTests.LinkedSource>()
                .LoadLinkPolymorphic(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes.Include<PolymorphicSubLinkedSourceTests.WebPageReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                            "web-page",
                            _ => new PolymorphicSubLinkedSourceTests.WebPageReference()
                        )
                        .Include<PolymorphicSubLinkedSourceTests.WebPageReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                            "web-page",
                            _ => new PolymorphicSubLinkedSourceTests.WebPageReference()
                        )
                );

            var ex = Assert.Throws<LinkItException>(act);
            Assert.Contains("LinkedSource", ex.Message);
            Assert.Contains("linkedSource.Target", ex.Message);
            Assert.Contains("web-page", ex.Message);
        }
    }
}