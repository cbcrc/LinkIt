#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Linq;
using LinkIt.ConfigBuilders;
using LinkIt.Core;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using LinkIt.TopologicalSorting;
using Xunit;

namespace LinkIt.Tests.TopologicalSorting
{
    public class SimplestRootLinkedSourceTest
    {
        private LoadLinkProtocol _sut;

        public SimplestRootLinkedSourceTest()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>();
            _sut = (LoadLinkProtocol) loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public void CreateDependencyGraph()
        {
            var actual = _sut.CreateDependencyGraph(typeof(LinkedSource));

            Assert.Equal(1, actual.DependencyCount);

            var dependency = actual.Dependencies.First();
            Assert.Equal(typeof(LinkedSource), dependency.LinkedSourceType);
            Assert.Equal(typeof(Model), dependency.Type);
        }

        [Fact]
        public void ParseLoadingLevels()
        {
            var dependencyGraph = _sut.CreateDependencyGraph(typeof(LinkedSource));

            var actual = TopologicalSort.For(dependencyGraph).GetLoadingLevels();;

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