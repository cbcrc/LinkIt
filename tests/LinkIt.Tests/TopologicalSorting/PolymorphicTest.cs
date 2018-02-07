// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using FluentAssertions;
using LinkIt.ConfigBuilders;
using LinkIt.Core;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using LinkIt.TopologicalSorting;
using Xunit;

namespace LinkIt.Tests.TopologicalSorting
{
    public class PolymorphicTest
    {
        private LoadLinkProtocol _sut;

        public PolymorphicTest()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkPolymorphic(
                    linkedSource => linkedSource.Model.PolyRef,
                    linkedSource => linkedSource.Poly,
                    link => link.Kind,
                    includes => includes.Include<PersonLinkedSource>().AsNestedLinkedSourceById(
                            "person-nested",
                            link => (string) link.Value)
                        .Include<PersonLinkedSource>().AsNestedLinkedSourceFromModel(
                            "person-sub",
                            link => (Person) link.Value)
                        .Include<Image>().AsReferenceById(
                            "img",
                            link => (string) link.Value)
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);
            _sut = (LoadLinkProtocol) loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public void ParseLoadingLevels()
        {
            var dependencyGraph = _sut.CreateDependencyGraph(typeof(LinkedSource));

            var actual = dependencyGraph.Sort().GetLoadingLevels();

            Type[][] expected = { new[] { typeof(Model) }, new[] { typeof(Person) }, new[] { typeof(Image) } };

            actual.Should().BeEquivalentTo(expected);
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public object Poly { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public PolymorphicRef PolyRef { get; set; }
        }

        public class PolymorphicRef
        {
            public string Kind { get; set; }
            public object Value { get; set; }
        }
    }
}