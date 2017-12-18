#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using NUnit.Framework;

namespace LinkIt.Tests.Core.Polymorphic {
    public class PolymorphicSubLinkedSource_InvalidConfigTests {
        [Fact]
        public void LoadLink_PolymorphicSubLinkedSourceWithWrongLinkedSourceModelType_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            TestDelegate act = () => loadLinkProtocolBuilder.For<PolymorphicSubLinkedSourceTests.LinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes
                        .Include<PolymorphicSubLinkedSourceTests.WebPageReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                            "web-page",
                            link => "a string is not a WebPageReference"
                        )
                );

            Assert.That(
                act,
                Throws.ArgumentException
                    .With.Message.Contains("LinkedSource/Target").And
                    .With.Message.Contains("WebPageReference").And
                    .With.Message.Contains("getNestedLinkedSourceModel")
            );
        }

        [Fact]
        public void LoadLink_WithDiscriminantDuplicate_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            TestDelegate act = () => loadLinkProtocolBuilder.For<PolymorphicSubLinkedSourceTests.LinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes
                        .Include<PolymorphicSubLinkedSourceTests.WebPageReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                            "web-page",
                            link => new PolymorphicSubLinkedSourceTests.WebPageReference()
                        )
                        .Include<PolymorphicSubLinkedSourceTests.WebPageReferenceLinkedSource>().AsNestedLinkedSourceFromModel(
                            "web-page",
                            link => new PolymorphicSubLinkedSourceTests.WebPageReference()
                        )
                );

            Assert.That(
                act,
                Throws.ArgumentException
                    .With.Message.Contains("LinkedSource/Target").And
                    .With.Message.Contains("web-page")
            );
        }
    }
}
