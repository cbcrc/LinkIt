using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkConfig_WithCycleTests
    {
        [Test]
        public void CreateLoadLinkConfig_WithCycleCausedByReference_ShouldThrow()
        {
            TestDelegate act = () => new LoadLinkConfig(
                new List<ILoadLinkExpression>
                {
                    new RootLoadLinkExpression<CycleInReferenceLinkedSource, DirectCycle, string>(),
                    new ReferenceLoadLinkExpression<CycleInReferenceLinkedSource, DirectCycle, string>(
                        linkedSource => linkedSource.Model.ParentId,
                        (linkedSource, reference) => linkedSource.Parent = reference
                        )
                }
            );

            Assert.That(
                act,
                Throws.ArgumentException.With
                    .Message.ContainsSubstring("cycle").And
                    .Message.ContainsSubstring("DirectCycle")
                );
        }

        [Test]
        public void CreateLoadLinkConfig_WithCycleCausedByDirectNestedLinkedSource_ShouldThrow() {
            TestDelegate act = () => new LoadLinkConfig(
                new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<DirectCycleInNestedLinkedSource, DirectCycle, string>(),
                    new NestedLinkedSourceLoadLinkExpression<DirectCycleInNestedLinkedSource, DirectCycleInNestedLinkedSource, DirectCycle, string>(
                        linkedSource => linkedSource.Model.ParentId,
                        (linkedSource, nestedLinkedSource) => linkedSource.Parent = nestedLinkedSource
                        )
                }
            );

            Assert.That(
                act,
                Throws.ArgumentException.With
                    .Message.ContainsSubstring("cycle").And
                    .Message.ContainsSubstring("DirectCycle")
                );
        }

        [Test]
        public void CreateLoadLinkConfig_WithCycleCausedByIndirectNestedLinkedSource_ShouldThrow() {
            TestDelegate act = () => new LoadLinkConfig(
                new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<IndirectCycleLevel0LinkedSource, IndirectCycleLevel0, string>(),
                    new NestedLinkedSourceLoadLinkExpression<IndirectCycleLevel0LinkedSource, IndirectCycleLevel1LinkedSource, IndirectCycleLevel1, string>(
                        linkedSource => linkedSource.Model.Level1Id,
                        (linkedSource, nestedLinkedSource) => linkedSource.Level1 = nestedLinkedSource),
                    new NestedLinkedSourceLoadLinkExpression<IndirectCycleLevel1LinkedSource, IndirectCycleLevel0LinkedSource, IndirectCycleLevel0, string>(
                        linkedSource => linkedSource.Model.Level0Id,
                        (linkedSource, nestedLinkedSource) => linkedSource.Level0 = nestedLinkedSource
                        )
                }
            );

            Assert.That(
                act,
                Throws.ArgumentException.With
                    .Message.ContainsSubstring("cycle").And
                    .Message.ContainsSubstring("IndirectCycleLevel0")
                );
        }

        public class CycleInReferenceLinkedSource: ILinkedSource<DirectCycle>
        {
            public DirectCycle Model { get; set; }
            public DirectCycle Parent { get; set; }
        }

        public class DirectCycleInNestedLinkedSource : ILinkedSource<DirectCycle> {
            public DirectCycle Model { get; set; }
            public DirectCycleInNestedLinkedSource Parent { get; set; }
        }

        public class DirectCycle {
            public string Id { get; set; }
            public string ParentId { get; set; }
        }

        public class IndirectCycleLevel0LinkedSource : ILinkedSource<IndirectCycleLevel0> {
            public IndirectCycleLevel0 Model { get; set; }
            public IndirectCycleLevel1LinkedSource Level1 { get; set; }
        }

        public class IndirectCycleLevel1LinkedSource : ILinkedSource<IndirectCycleLevel1> {
            public IndirectCycleLevel1 Model { get; set; }
            public IndirectCycleLevel0LinkedSource Level0 { get; set; }
        }

        public class IndirectCycleLevel0 {
            public string Id { get; set; }
            public string Level1Id { get; set; }
        }

        public class IndirectCycleLevel1 {
            public string Id { get; set; }
            public string Level0Id { get; set; }
        }
    }
}
