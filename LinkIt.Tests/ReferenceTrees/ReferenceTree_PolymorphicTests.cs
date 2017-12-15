#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using FluentAssertions;
using LinkIt.ConfigBuilders;
using LinkIt.Core;
using LinkIt.PublicApi;
using LinkIt.ReferenceTrees;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.ReferenceTrees
{
    public class ReferenceTree_PolymorphicTest
    {
        private LoadLinkProtocol _sut;

        public ReferenceTree_PolymorphicTest()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .PolymorphicLoadLink(
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
        public void CreateRootReferenceTree()
        {
            var actual = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            var expected = GetExpectedReferenceTree();

            actual.Should().BeEquivalentTo(expected);
        }

        private static ReferenceTree GetExpectedReferenceTree()
        {
            var expected = new ReferenceTree(typeof(Model), $"root of {typeof(LinkedSource)}", null);

            var child1 = new ReferenceTree(typeof(Person), $"{typeof(LinkedSource)}/{nameof(LinkedSource.Poly)}", expected);
            new ReferenceTree(typeof(Image), $"{typeof(PersonLinkedSource)}/{nameof(PersonLinkedSource.SummaryImage)}", child1);

            new ReferenceTree(typeof(Image), $"{typeof(LinkedSource)}/{nameof(LinkedSource.Poly)}", expected);
            new ReferenceTree(typeof(Image), $"{typeof(PersonLinkedSource)}/{nameof(PersonLinkedSource.SummaryImage)}", expected);

            return expected;
        }

        [Fact]
        public void ParseLoadingLevels()
        {
            var rootReferenceTree = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            var actual = rootReferenceTree.ParseLoadingLevels();

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