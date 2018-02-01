using System;
using System.Collections.Generic;
using FluentAssertions;
using LinkIt.ConfigBuilders;
using LinkIt.Core;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using LinkIt.TopologicalSorting;
using Xunit;

namespace LinkIt.Tests.TopologicalSorting
{
    public class SideEffectBetweenNestedLinkedSourceTests
    {
        private LoadLinkProtocol _sut;

        public SideEffectBetweenNestedLinkedSourceTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.PersonId,
                    linkedSource => linkedSource.Person
                )
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.PersonGroupId,
                    linkedSource => linkedSource.PersonGroup
                );
            loadLinkProtocolBuilder.For<PersonGroupLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PersonIds,
                    linkedSource => linkedSource.People);


            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);
            _sut = (LoadLinkProtocol) loadLinkProtocolBuilder.Build(
                () => null //not required
            );
        }

        [Fact]
        public void ParseLoadingLevels()
        {
            var dependencyGraph = _sut.CreateDependencyGraph(typeof(LinkedSource));

            var actual = dependencyGraph.Sort().GetLoadingLevels();

            Type[][] expected =
            {
                new[] { typeof(Model) },
                new[] { typeof(PersonGroup) },
                new[] { typeof(Person) },
                new[] { typeof(Image) }
            };

            actual.Should().BeEquivalentTo(expected, because: "Model types for linked sources should be grouped with other dependencies of the same type.");
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public PersonLinkedSource Person { get; set; }
            public PersonGroupLinkedSource PersonGroup { get; set; }
            public Model Model { get; set; }
        }

        public class PersonGroupLinkedSource : ILinkedSource<PersonGroup>
        {
            public List<Person> People { get; set; }
            public PersonGroup Model { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public string PersonId { get; set; }
            public int PersonGroupId { get; set; }
        }

        public class PersonGroup
        {
            public int Id { get; set; }
            public List<string> PersonIds { get; set; }
        }
    }
}