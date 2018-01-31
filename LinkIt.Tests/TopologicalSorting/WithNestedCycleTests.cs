using System;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.TopologicalSorting
{
    public class WithNestedCycleTests
    {
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

        [Fact]
        public void CreateLoadLinkConfig_WithCycleWithinLinkedSource_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SelfReferencingLinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.ParentId,
                    linkedSource => linkedSource.Parent);

            Action act = () => loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            var exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal($"Recursive dependency detected for type {{ {nameof(SelfReferencingLinkedSource)} }}.", exception.Message);
        }

        [Fact]
        public void CreateLoadLinkConfig_WithCycleCausedByNestedLinkedSourceReference_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<ParentLinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.ChildId,
                    linkedSource => linkedSource.Child
                );
            loadLinkProtocolBuilder.For<ChildLinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.ParentId,
                    linkedSource => linkedSource.Parent
                );

            Action act = () => loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            var exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal($"Recursive dependency detected for type {{ {nameof(ParentLinkedSource)} }}.", exception.Message);
        }

        [Fact]
        public void CreateLoadLinkConfig_WithCycleCausedByIndirectLinkedSources_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<IndirectCycleLinkedSourceA>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.ModelBId,
                    linkedSource => linkedSource.ModelB
                );
            loadLinkProtocolBuilder.For<IndirectCycleLinkedSourceB>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.ModelCId,
                    linkedSource => linkedSource.ModelC
                );
            loadLinkProtocolBuilder.For<IndirectCycleLinkedSourceC>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.ModelAId,
                    linkedSource => linkedSource.ModelA
                );

            Action act = () => loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            var exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal($"Recursive dependency detected for type {{ {nameof(IndirectCycleLinkedSourceA)} }}.", exception.Message);
        }

        private class CycleInReferenceLinkedSource : ILinkedSource<DirectCycle>
        {
            public DirectCycle Model { get; set; }
            public DirectCycle Parent { get; set; }
        }

        private class SelfReferencingLinkedSource : ILinkedSource<DirectCycle>
        {
            public DirectCycle Model { get; set; }
            public SelfReferencingLinkedSource Parent { get; set; }
        }

        private class DirectCycle
        {
            public string Id { get; set; }
            public string ParentId { get; set; }
        }

        private class ParentLinkedSource : ILinkedSource<ParentModel>
        {
            public ParentModel Model { get; set; }
            public ChildLinkedSource Child { get; set; }
        }

        private class ChildLinkedSource : ILinkedSource<ChildModel>
        {
            public ChildModel Model { get; set; }
            public ParentLinkedSource Parent { get; set; }
        }

        private class ParentModel
        {
            public string Id { get; set; }
            public string ChildId { get; set; }
        }

        private class ChildModel
        {
            public string Id { get; set; }
            public string ParentId { get; set; }
        }

        private class IndirectCycleLinkedSourceA : ILinkedSource<ModelA>
        {
            public ModelA Model { get; set; }
            public IndirectCycleLinkedSourceB ModelB { get; set; }
        }

        private class IndirectCycleLinkedSourceB : ILinkedSource<ModelB>
        {
            public ModelB Model { get; set; }
            public IndirectCycleLinkedSourceC ModelC { get; set; }
        }

        private class IndirectCycleLinkedSourceC : ILinkedSource<ModelC>
        {
            public ModelC Model { get; set; }
            public IndirectCycleLinkedSourceA ModelA { get; set; }
        }

        private class ModelA
        {
            public string Id { get; set; }
            public string ModelBId { get; set; }
        }

        private class ModelB
        {
            public string Id { get; set; }
            public string ModelCId { get; set; }
        }

        private class ModelC
        {
            public string Id { get; set; }
            public string ModelAId { get; set; }
        }
    }
}