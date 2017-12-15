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
    public class ReferenceTree_ReferenceTest
    {
        private LoadLinkProtocol _sut;

        public ReferenceTree_ReferenceTest()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PersonOneId,
                    linkedSource => linkedSource.PersonOne)
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PersonTwoId,
                    linkedSource => linkedSource.PersonTwo
                );
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
            new ReferenceTree(typeof(Person), $"{typeof(LinkedSource)}/{nameof(LinkedSource.PersonOne)}", expected);
            new ReferenceTree(typeof(Person), $"{typeof(LinkedSource)}/{nameof(LinkedSource.PersonTwo)}", expected);

            return expected;
        }

        [Fact]
        public void ParseLoadingLevels()
        {
            var rootReferenceTree = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            var actual = rootReferenceTree.ParseLoadingLevels();

            Type[][] expected = { new[] { typeof(Model) }, new[] { typeof(Person) } };

            actual.Should().BeEquivalentTo(expected);
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public Person PersonOne { get; set; }
            public Person PersonTwo { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public string PersonOneId { get; set; }
            public string PersonTwoId { get; set; }
        }
    }
}