#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

//using System;
using System;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
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

            var exception = Assert.Throws<NotSupportedException>(act);
            Assert.Contains("DirectCycleInNestedLinkedSource", exception.Message);
            Assert.IsType<NotSupportedException>(exception.InnerException);
            Assert.StartsWith("Recursive load link", exception.InnerException.Message);
            Assert.Contains("root", exception.InnerException.Message);
            Assert.Contains("Parent", exception.InnerException.Message);
            Assert.Contains("DirectCycle", exception.InnerException.Message);
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

            var exception = Assert.Throws<NotSupportedException>(act);
            Assert.Contains("IndirectCycleLevel0LinkedSource", exception.Message);
            Assert.IsType<NotSupportedException>(exception.InnerException);
            Assert.StartsWith("Recursive load link", exception.InnerException.Message);
            Assert.Contains("root", exception.InnerException.Message);
            Assert.Contains("Level0", exception.InnerException.Message);
            Assert.Contains("IndirectCycleLevel0", exception.InnerException.Message);
        }

        [Fact]
        public void CreateLoadLinkConfig_WithCycleCausedByReference_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<CycleInReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ParentId,
                    linkedSource => linkedSource.Parent);

            Action act = () => loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            var exception = Assert.Throws<NotSupportedException>(act);
            Assert.Contains("CycleInReferenceLinkedSource", exception.Message);
            Assert.IsType<NotSupportedException>(exception.InnerException);
            Assert.StartsWith("Recursive load link", exception.InnerException.Message);
            Assert.Contains("root", exception.InnerException.Message);
            Assert.Contains("Parent", exception.InnerException.Message);
            Assert.Contains("DirectCycle", exception.InnerException.Message);
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