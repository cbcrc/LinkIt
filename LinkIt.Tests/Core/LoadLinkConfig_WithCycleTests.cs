using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.Shared;
using NUnit.Framework;

namespace LinkIt.Tests.Core {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkConfig_WithCycleTests
    {
        [Test]
        public void CreateLoadLinkConfig_WithCycleCausedByReference_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<CycleInReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ParentId,
                    linkedSource => linkedSource.Parent
                );

            TestDelegate act = () => loadLinkProtocolBuilder.Build(()=>new ReferenceLoaderStub());

            Assert.That(
                act,
                Throws.Exception
                    .With.Message.ContainsSubstring("CycleInReferenceLinkedSource").And
                    .With.InnerException
                        .With.Message.ContainsSubstring("Recursive load link").And
                        .With.Message.ContainsSubstring("root").And
                        .With.Message.ContainsSubstring("Parent").And
                        .With.Message.ContainsSubstring("DirectCycle")
                );
        }

        [Test]
        public void CreateLoadLinkConfig_WithCycleCausedByDirectNestedLinkedSource_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<DirectCycleInNestedLinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.ParentId,
                    linkedSource => linkedSource.Parent
                );

            TestDelegate act = () => loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            Assert.That(
                act,
                Throws.Exception
                    .With.Message.ContainsSubstring("DirectCycleInNestedLinkedSource").And
                    .With.InnerException
                        .With.Message.ContainsSubstring("Recursive load link").And
                        .With.Message.ContainsSubstring("root").And
                        .With.Message.ContainsSubstring("Parent").And
                        .With.Message.ContainsSubstring("DirectCycle")
                );
        }

        [Test]
        public void CreateLoadLinkConfig_WithCycleCausedByIndirectNestedLinkedSource_ShouldThrow() {
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

            TestDelegate act = () => loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            Assert.That(
               act,
               Throws.Exception
                   .With.Message.ContainsSubstring("IndirectCycleLevel0LinkedSource").And
                   .With.InnerException
                       .With.Message.ContainsSubstring("Recursive load link").And
                       .With.Message.ContainsSubstring("root").And
                       .With.Message.ContainsSubstring("Level0").And
                       .With.Message.ContainsSubstring("IndirectCycleLevel0")
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
