// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PolymorphicMixtedTests
    {
        private readonly ILoadLinkProtocol _sut;

        public PolymorphicMixtedTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkPolymorphic(
                    linkedSource => linkedSource.Model.TargetReference,
                    linkedSource => linkedSource.Target,
                    link => link.GetType(),
                    includes => includes.Include<Person>().AsReferenceById(
                            typeof(int),
                            link => link.ToString()
                        )
                        .Include<PersonLinkedSource>().AsNestedLinkedSourceById(
                            typeof(string),
                            link => link.ToString()
                        )
                        .Include<PersonLinkedSource>().AsNestedLinkedSourceFromModel(
                            typeof(Person),
                            link => (Person) link
                        )
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_MixedPolymorphicAsReferenceAsync()
        {
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(
                new Model
                {
                    Id = "1",
                    TargetReference = 1
                }
            );

            var person = Assert.IsType<Person>(actual.Target);
            Assert.Equal("1", person.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_MixedPolymorphicAsNestedLinkedSourceAsync()
        {
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(
                new Model
                {
                    Id = "1",
                    TargetReference = "nested"
                }
            );

            var personLinkedSource = Assert.IsType<PersonLinkedSource>(actual.Target);
            Assert.Equal("nested", personLinkedSource.Model.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_MixedPolymorphicAsSubLinkedSourceAsync()
        {
            var person = new Person
            {
                Id = "as-sub-linked-source",
                Name = "The Name",
                SummaryImageId = "the-id"
            };
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(
                new Model
                {
                    Id = "1",
                    TargetReference = person
                }
            );

            var personLinkedSource = Assert.IsType<PersonLinkedSource>(actual.Target);
            Assert.Same(person, personLinkedSource.Model);
            Assert.Equal(person.SummaryImageId, personLinkedSource.SummaryImage.Id);
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public object Target { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public object TargetReference { get; set; }
        }
    }
}