// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PolymorphicSubLinkedSourcesTests
    {
        private readonly ILoadLinkProtocol _sut;

        public PolymorphicSubLinkedSourcesTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<WithPolymorphicSubLinkedSource>()
                .LoadLinkPolymorphicList(
                    linkedSource => linkedSource.Model.Subs,
                    linkedSource => linkedSource.Subs,
                    link => link.GetType(),
                    includes => includes.Include<SubContentWithImageLinkedSource>().AsNestedLinkedSourceFromModel(
                            typeof(SubContentWithImage),
                            link => (SubContentWithImage) link,
                            (linkedSource, referenceIndex, childLinkedSource) =>
                                childLinkedSource.Contextualization = linkedSource.Model.Contextualizations[referenceIndex]
                        )
                        .Include<SubContentWithoutReferencesLinkedSource>().AsNestedLinkedSourceFromModel(
                            typeof(SubContentWithoutReferences),
                            link => (SubContentWithoutReferences) link,
                            (linkedSource, referenceIndex, childLinkedSource) =>
                                childLinkedSource.Contextualization = linkedSource.Model.Contextualizations[referenceIndex]
                        )
                );

            loadLinkProtocolBuilder.For<SubContentWithImageLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ImageId,
                    linkedSource => linkedSource.Image);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_SubContentWithoutReferencesAsync()
        {
            var actual = await _sut.LoadLink<WithPolymorphicSubLinkedSource>().FromModelAsync(
                new WithPolymorphicSubLinkedSourceContent
                {
                    Id = "1",
                    Contextualizations = new[]
                    {
                        "first-contextualization",
                        "middle-contextualization",
                        "last-contextualization"
                    }.ToList(),
                    Subs = new List<IPolymorphicModel>
                    {
                        new SubContentWithImage
                        {
                            Id = "a",
                            ImageId = "i-a"
                        },
                        null,
                        new SubContentWithoutReferences
                        {
                            Id = "b",
                            Title = "sub-b"
                        }
                    }
                }
            );

            Assert.Collection(
                actual.Subs,
                link =>
                {
                    var linkedSource = Assert.IsType<SubContentWithImageLinkedSource>(link);
                    Assert.Equal("a", linkedSource.Model.Id);
                    Assert.Equal("i-a", linkedSource.Image.Id);
                    Assert.Equal("first-contextualization", linkedSource.Contextualization);
                },
                link =>
                {
                    var linkedSource = Assert.IsType<SubContentWithoutReferencesLinkedSource>(link);
                    Assert.Equal("b", linkedSource.Model.Id);
                    Assert.Equal("last-contextualization", linkedSource.Contextualization);
                }
            );
        }

        public class WithPolymorphicSubLinkedSource : ILinkedSource<WithPolymorphicSubLinkedSourceContent>
        {
            public List<IPolymorphicSource> Subs { get; set; }
            public WithPolymorphicSubLinkedSourceContent Model { get; set; }
        }

        public class SubContentWithImageLinkedSource : ILinkedSource<SubContentWithImage>, IPolymorphicSource
        {
            public string Contextualization { get; set; }
            public Image Image { get; set; }
            public SubContentWithImage Model { get; set; }
        }

        public class SubContentWithoutReferencesLinkedSource : ILinkedSource<SubContentWithoutReferences>, IPolymorphicSource
        {
            public string Contextualization { get; set; }
            public SubContentWithoutReferences Model { get; set; }
        }

        public class WithPolymorphicSubLinkedSourceContent
        {
            public string Id { get; set; }
            public List<string> Contextualizations { get; set; }
            public List<IPolymorphicModel> Subs { get; set; }
        }

        public class SubContentWithImage : IPolymorphicModel
        {
            public string Id { get; set; }
            public string ImageId { get; set; }
        }

        public class SubContentWithoutReferences : IPolymorphicModel
        {
            public string Id { get; set; }
            public string Title { get; set; }
        }
    }
}