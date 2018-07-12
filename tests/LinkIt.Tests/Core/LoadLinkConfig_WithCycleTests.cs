// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

//using System;
using System;
using FluentAssertions;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using LinkIt.Tests.TopologicalSorting;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LoadLinkConfig_WithCycleTest
    {
        [Fact]
        public void CreateLoadLinkConfig_WithCycleCausedByDirectNestedLinkedSource_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<DirectCycleInNestedLinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.ParentId,
                    linkedSource => linkedSource.Parent);

            Action act = () => loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            var exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal($"Recursive dependency detected for type {{ {nameof(DirectCycleInNestedLinkedSource)} }}.", exception.Message);
        }

        [Fact]
        public void CreateLoadLinkConfig_WithCycleCausedByIndirectNestedLinkedSource_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<IndirectCycleLevel0LinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.Level1Id,
                    linkedSource => linkedSource.Level1
                );
            loadLinkProtocolBuilder.For<IndirectCycleLevel1LinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.Level0Id,
                    linkedSource => linkedSource.Level0
                );

            Action act = () => loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            var exception = Assert.Throws<InvalidOperationException>(act);
            exception.Message.Should().BeOneOf(
                $"Recursive dependency detected for type {{ {nameof(IndirectCycleLevel0LinkedSource)} }}.",
                $"Recursive dependency detected for type {{ {nameof(IndirectCycleLevel1LinkedSource)} }}.");
        }

        [Fact]
        public void CreateLoadLinkConfig_WithCycleCausedByReference_ShouldNotThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<CycleInReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ParentId,
                    linkedSource => linkedSource.Parent);

            var loadLinkProtocol = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            Assert.NotNull(loadLinkProtocol);
        }

        public class CycleInReferenceLinkedSource : ILinkedSource<DirectCycle>
        {
            public DirectCycle Parent { get; set; }
            public DirectCycle Model { get; set; }
        }

        public class DirectCycleInNestedLinkedSource : ILinkedSource<DirectCycle>
        {
            public DirectCycleInNestedLinkedSource Parent { get; set; }
            public DirectCycle Model { get; set; }
        }

        public class DirectCycle
        {
            public string Id { get; set; }
            public string ParentId { get; set; }
        }

        public class IndirectCycleLevel0LinkedSource : ILinkedSource<IndirectCycleLevel0>
        {
            public IndirectCycleLevel1LinkedSource Level1 { get; set; }
            public IndirectCycleLevel0 Model { get; set; }
        }

        public class IndirectCycleLevel1LinkedSource : ILinkedSource<IndirectCycleLevel1>
        {
            public IndirectCycleLevel0LinkedSource Level0 { get; set; }
            public IndirectCycleLevel1 Model { get; set; }
        }

        public class IndirectCycleLevel0
        {
            public string Id { get; set; }
            public string Level1Id { get; set; }
        }

        public class IndirectCycleLevel1
        {
            public string Id { get; set; }
            public string Level0Id { get; set; }
        }
    }
}
