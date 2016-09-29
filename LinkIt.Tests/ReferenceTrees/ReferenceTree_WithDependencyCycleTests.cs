using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;

namespace LinkIt.Tests.ReferenceTrees {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTree_WithDependencyCycleTests {
        [Test]
        public void CreateLoadLinkConfig_WithCycleCausedByReference_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<DependencyCycleLinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.AId,
                    linkedSource => linkedSource.A
                )
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.BId,
                    linkedSource => linkedSource.B
                )
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.CId,
                    linkedSource => linkedSource.C
                );
            loadLinkProtocolBuilder.For<ALinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.BId,
                    linkedSource => linkedSource.B
                );
            loadLinkProtocolBuilder.For<BLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.CId,
                    linkedSource => linkedSource.C
                );
            loadLinkProtocolBuilder.For<CLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.AId,
                    linkedSource => linkedSource.A
                );

            TestDelegate act = () => loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            Assert.That(
                act,
                Throws.Exception
                    .With.Message.ContainsSubstring("DependencyCycleLinkedSource").And
                    .With.InnerException
                        .With.Message.ContainsSubstring("Recursive load link").And
                        .With.Message.ContainsSubstring("Cannot infer which reference type should be loaded first")
                );
        }

        public class DependencyCycleLinkedSource : ILinkedSource<DependencyCycle> {
            public DependencyCycle Model { get; set; }
            public ALinkedSource A { get; set; }
            public BLinkedSource B { get; set; }
            public CLinkedSource C { get; set; }
        }

        public class ALinkedSource : ILinkedSource<A> {
            public A Model { get; set; }
            public B B { get; set; }
        }

        public class BLinkedSource : ILinkedSource<B> {
            public B Model { get; set; }
            public C C { get; set; }
        }

        public class CLinkedSource : ILinkedSource<C> {
            public C Model { get; set; }
            public A A { get; set; }
        }

        public class DependencyCycle {
            public string Id { get; set; }
            public string AId { get; set; }
            public string BId { get; set; }
            public string CId { get; set; }
        }

        public class A {
            public string Id { get; set; }
            public string BId { get; set; }
        }

        public class B {
            public string Id { get; set; }
            public string CId { get; set; }
        }

        public class C {
            public string Id { get; set; }
            public string AId { get; set; }
        }
    }
}
