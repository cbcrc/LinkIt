// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PolymorphicReferenceTests
    {
        private ILoadLinkProtocol _sut;

        public PolymorphicReferenceTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkPolymorphic(
                    linkedSource => linkedSource.Model.Target,
                    linkedSource => linkedSource.Target,
                    link => link.Type,
                    includes => includes.Include<Image>().AsReferenceById(
                            "image",
                            link => link.Id
                        )
                        .Include<Person>().AsReferenceById(
                            "person",
                            link => link.Id
                        )
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_PolymorphicReferenceWithImageAsync()
        {
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(
                new Model
                {
                    Id = "1",
                    Target = new PolymorphicReference
                    {
                        Type = "image",
                        Id = "a"
                    }
                }
            );

            var image = Assert.IsType<Image>(actual.Target);
            Assert.Equal("a", image.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_PolymorphicReferenceWithPersonAsync()
        {
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(
                new Model
                {
                    Id = "1",
                    Target = new PolymorphicReference
                    {
                        Type = "person",
                        Id = "a"
                    }
                }
            );

            var person = Assert.IsType<Person>(actual.Target);
            Assert.Equal("a", person.Id);
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public object Target { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public PolymorphicReference Target { get; set; }
        }

        public class PolymorphicReference
        {
            public string Type { get; set; }
            public string Id { get; set; }
        }
    }
}