// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    public class NestedLinkedSourceTest
    {
        private LoadLinkProtocol _sut;
        public NestedLinkedSourceTest()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PreImageId,
                    linkedSource => linkedSource.PreImage)
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.PersonId,
                    linkedSource => linkedSource.Person
                )
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PostImageId,
                    linkedSource => linkedSource.PostImage);
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

            var actual = TopologicalSort.For(dependencyGraph).GetLoadingLevels();

            Type[][] expected = { new[] { typeof(Model) }, new[] { typeof(Person) }, new[] { typeof(Image) } };

            actual.Should().BeEquivalentTo(expected);
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public Image PreImage { get; set; }
            public PersonLinkedSource Person { get; set; }
            public Image PostImage { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public string PreImageId { get; set; }
            public string PersonId { get; set; }
            public string PostImageId { get; set; }
        }
    }
}