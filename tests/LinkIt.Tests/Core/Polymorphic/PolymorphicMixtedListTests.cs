// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Polymorphic
{
    public class PolymorphicMixtedListTests
    {
        private ILoadLinkProtocol _sut;

        public PolymorphicMixtedListTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkPolymorphicList(
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
        public async System.Threading.Tasks.Task LoadLink_PolymorphicMixedListAsync()
        {
            var model = new Model
            {
                Id = "1",
                TargetReference = new List<object>
                {
                    1,
                    "nested",
                    new Person
                    {
                        Id = "as-sub-linked-source",
                        Name = "The Name",
                        SummaryImageId = "the-id"
                    }
                }
            };
            var actual = await _sut.LoadLink<LinkedSource>().FromModelAsync(model);

            Assert.Collection(
                actual.Target,
                target =>
                {
                    var person = Assert.IsType<Person>(target);
                    Assert.Equal("1", person.Id);
                },
                target =>
                {
                    var personLinkedSource = Assert.IsType<PersonLinkedSource>(target);
                    Assert.Equal("nested", personLinkedSource.Model.Id);
                },
                target =>
                {
                    var personLinkedSource = Assert.IsType<PersonLinkedSource>(target);
                    Assert.Same(((Person)model.TargetReference[2]), personLinkedSource.Model);
                    Assert.Equal("the-id", personLinkedSource.SummaryImage.Id);
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
            public List<object> TargetReference { get; set; }
        }
    }
}