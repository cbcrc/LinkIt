#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PolymorphicReferencesTests
    {
        private ILoadLinkProtocol _sut;

        public PolymorphicReferencesTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLinkForList(
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
                    Target = new List<PolymorphicReference>
                    {
                        new PolymorphicReference
                        {
                            Type = "person",
                            Id = "a"
                        },
                        new PolymorphicReference
                        {
                            Type = "image",
                            Id = "b"
                        }
                    }
                }
            );

            Assert.Collection(
                actual.Target,
                target =>
                {
                    var person = Assert.IsType<Person>(target);
                    Assert.Equal("a", person.Id);
                },
                target =>
                {
                    var image = Assert.IsType<Image>(target);
                    Assert.Equal("b", image.Id);
                }
            );
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public List<object> Target { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public List<PolymorphicReference> Target { get; set; }
        }

        public class PolymorphicReference
        {
            public string Type { get; set; }
            public string Id { get; set; }
        }
    }
}