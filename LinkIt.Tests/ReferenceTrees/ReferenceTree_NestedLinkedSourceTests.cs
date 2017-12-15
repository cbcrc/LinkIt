// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using LinkIt.ConfigBuilders;
using LinkIt.Core;
using LinkIt.PublicApi;
using LinkIt.ReferenceTrees;
using LinkIt.Shared;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.ReferenceTrees
{
    public class ReferenceTree_NestedLinkedSourceTest
    {
        private LoadLinkProtocol _sut;
        public ReferenceTree_NestedLinkedSourceTest()
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
        public void CreateRootReferenceTree()
        {
            var actual = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            var expected = GetExpectedReferenceTree();

            actual.Should().BeEquivalentTo(expected);
        }

        private static ReferenceTree GetExpectedReferenceTree()
        {
            var expected = new ReferenceTree(typeof(Model), $"root of {typeof(LinkedSource)}", null);
            new ReferenceTree(typeof(Image), $"{typeof(LinkedSource)}/{nameof(LinkedSource.PreImage)}", expected);
            var child2 = new ReferenceTree(typeof(Person), $"{typeof(LinkedSource)}/{nameof(LinkedSource.Person)}", expected);
            new ReferenceTree(typeof(Image), $"{typeof(PersonLinkedSource)}/{nameof(PersonLinkedSource.SummaryImage)}", child2);
            new ReferenceTree(typeof(Image), $"{typeof(LinkedSource)}/{nameof(LinkedSource.PostImage)}", expected);

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