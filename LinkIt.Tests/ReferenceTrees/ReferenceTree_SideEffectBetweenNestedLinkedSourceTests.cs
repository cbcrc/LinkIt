using System;
using System.Collections.Generic;
using FluentAssertions;
using LinkIt.ConfigBuilders;
using LinkIt.Core;
using LinkIt.PublicApi;
using LinkIt.ReferenceTrees;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.ReferenceTrees
{
    public class ReferenceTree_SideEffectBetweenNestedLinkedSourceTests
    {
        private LoadLinkProtocol _sut;

        public ReferenceTree_SideEffectBetweenNestedLinkedSourceTests()
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
        public void CreateRootReferenceTree()
        {
            var actual = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            var expected = GetExpectedReferenceTree();

            actual.Should().BeEquivalentTo(expected);
        }

        private static ReferenceTree GetExpectedReferenceTree()
        {
            var expected = new ReferenceTree(typeof(Model), $"root of {typeof(LinkedSource)}", null);

            var child1 = new ReferenceTree(typeof(Person), $"{typeof(LinkedSource)}/{nameof(LinkedSource.Person)}", expected);
            new ReferenceTree(typeof(Image), $"{typeof(PersonLinkedSource)}/{nameof(PersonLinkedSource.SummaryImage)}", child1);

            var child2 = new ReferenceTree(typeof(PersonGroup), $"{typeof(LinkedSource)}/{nameof(LinkedSource.PersonGroup)}", expected);
            new ReferenceTree(typeof(Person), $"{typeof(PersonGroupLinkedSource)}/{nameof(PersonGroupLinkedSource.People)}", child2);

            return expected;
        }

        [Fact]
        public void ParseLoadingLevels()
        {
            var rootReferenceTree = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            var actual = rootReferenceTree.ParseLoadingLevels();

            Type[][] expected =
            {
                new[] { typeof(Model) },
                new[] { typeof(PersonGroup) },
                new[] { typeof(Person) },
                new[] { typeof(Image) }
            };

            actual.Should().BeEquivalentTo(expected);
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