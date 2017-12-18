#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;


namespace LinkIt.Tests.Core.Polymorphic {
    public class PolymorphicSubLinkedSourcesTests {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<WithPolymorphicSubLinkedSource>()
                .PolymorphicLoadLinkForList(
                    linkedSource => linkedSource.Model.Subs,
                    linkedSource => linkedSource.Subs,
                    link => link.GetType(),
                    includes => includes
                        .Include<SubContentWithImageLinkedSource>().AsNestedLinkedSourceFromModel(
                            typeof(SubContentWithImage),
                            link => (SubContentWithImage)link,
                            (linkedSource, referenceIndex, childLinkedSource) =>
                                childLinkedSource.Contextualization = linkedSource.Model.Contextualizations[referenceIndex]
                        )
                        .Include<SubContentWithoutReferencesLinkedSource>().AsNestedLinkedSourceFromModel(
                            typeof(SubContentWithoutReferences),
                            link => (SubContentWithoutReferences)link,
                            (linkedSource, referenceIndex, childLinkedSource) =>
                                childLinkedSource.Contextualization = linkedSource.Model.Contextualizations[referenceIndex]
                        )
                );

            loadLinkProtocolBuilder.For<SubContentWithImageLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ImageId,
                    linkedSource => linkedSource.Image
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public void LoadLink_SubContentWithoutReferences() {
            var actual = _sut.LoadLink<WithPolymorphicSubLinkedSource>().FromModel(
                new WithPolymorphicSubLinkedSourceContent {
                    Id = "1",
                    Contextualizations = new[]
                    {
                        "first-contextualization",
                        "middle-contextualization",
                        "last-contextualization",
                    }.ToList(),
                    Subs = new List<IPolymorphicModel>
                    {
                        new SubContentWithImage
                        {
                            Id="a",
                            ImageId = "i-a"
                        },
                        null,
                        new SubContentWithoutReferences
                        {
                            Id="b",
                            Title = "sub-b"
                        },
                    }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class WithPolymorphicSubLinkedSource : ILinkedSource<WithPolymorphicSubLinkedSourceContent> {
            public WithPolymorphicSubLinkedSourceContent Model { get; set; }
            public List<IPolymorphicSource> Subs { get; set; }
        }

        public class SubContentWithImageLinkedSource : ILinkedSource<SubContentWithImage>, IPolymorphicSource {
            public SubContentWithImage Model { get; set; }
            public string Contextualization { get; set; }
            public Image Image { get; set; }
        }

        public class SubContentWithoutReferencesLinkedSource : ILinkedSource<SubContentWithoutReferences>, IPolymorphicSource{
            public SubContentWithoutReferences Model { get; set; }
            public string Contextualization { get; set; }
        }

        public class WithPolymorphicSubLinkedSourceContent {
            public string Id { get; set; }
            public List<string> Contextualizations { get; set; }
            public List<IPolymorphicModel> Subs { get; set; }
        }

        public class SubContentWithImage : IPolymorphicModel {
            public string Id { get; set; }
            public string ImageId{ get; set; }
        }

        public class SubContentWithoutReferences : IPolymorphicModel {
            public string Id { get; set; }
            public string Title { get; set; }
        }
    }
}
