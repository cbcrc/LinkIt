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
    public class ReferenceTree_SimplestRootLinkedSourceTest
    {
        private LoadLinkProtocol _sut;

        public ReferenceTree_SimplestRootLinkedSourceTest()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>();
            _sut = (LoadLinkProtocol) loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public void CreateRootReferenceTree()
        {
            var actual = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            Assert.Equal(typeof(Model), actual.Node.ReferenceType);
            Assert.Equal($"root of {typeof(LinkedSource)}", actual.Node.LinkTargetId);
        }

        [Fact]
        public void ParseLoadingLevels()
        {
            var rootReferenceTree = _sut.CreateRootReferenceTree(typeof(LinkedSource));

            var actual = rootReferenceTree.ParseLoadingLevels();

            Assert.Equal(typeof(Model), actual[0][0]);
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public Model Model { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
        }
    }
}