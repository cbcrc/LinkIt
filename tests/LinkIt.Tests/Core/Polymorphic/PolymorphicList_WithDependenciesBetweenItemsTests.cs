// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PolymorphicList_WithDependenciesBetweenItemsTests
    {
        private ILoadLinkProtocol _sut;

        public PolymorphicList_WithDependenciesBetweenItemsTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkPolymorphicList(
                    linkedSource => linkedSource.Model.PolyLinks,
                    linkedSource => linkedSource.PolyLinks,
                    link => link.Type,
                    includes => includes.Include<Image>().AsReferenceById(
                            "image",
                            link => link.Id
                        )
                        .Include<PersonLinkedSource>().AsNestedLinkedSourceById(
                            "person",
                            link => link.Id
                        )
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_WithDependenciesBetweenItems_ShouldLink3ItemsAsync()
        {
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(
                new Model
                {
                    Id = "1",
                    PolyLinks = new List<Link>
                    {
                        new Link { Type = "image", Id = "before" },
                        new Link { Type = "person", Id = "the-person" },
                        new Link { Type = "image", Id = "after" }
                    }
                }
            );

            Assert.Collection(
                actual.PolyLinks,
                link =>
                {
                    var image = Assert.IsType<Image>(link);
                    Assert.Equal("before", image.Id);
                },
                link =>
                {
                    var personLinkedSource = Assert.IsType<PersonLinkedSource>(link);
                    Assert.Equal("the-person", personLinkedSource.Model.Id);
                },
                link =>
                {
                    var image = Assert.IsType<Image>(link);
                    Assert.Equal("after", image.Id);
                }
            );
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public List<object> PolyLinks { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public List<Link> PolyLinks { get; set; }
        }

        public class Link
        {
            public string Type { get; set; }
            public string Id { get; set; }
        }
    }
}