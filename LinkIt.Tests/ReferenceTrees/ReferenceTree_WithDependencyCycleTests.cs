using System;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.ReferenceTrees
{
    public class ReferenceTree_WithDependencyCycleTests
    {
        [Fact]
        public void CreateLoadLinkConfig_WithCycleCausedByReference_ShouldThrow()
        {
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

            Action act = () => loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());

            var exception = Assert.Throws<NotSupportedException>(act);
            Assert.Contains("DependencyCycleLinkedSource", exception.Message);
            Assert.IsType<NotSupportedException>(exception.InnerException);
            Assert.StartsWith("Recursive load link", exception.InnerException.Message);
            Assert.Contains("Cannot infer which reference type should be loaded first", exception.InnerException.Message);
        }

        public class DependencyCycleLinkedSource : ILinkedSource<DependencyCycle>
        {
            public ALinkedSource A { get; set; }
            public BLinkedSource B { get; set; }
            public CLinkedSource C { get; set; }
            public DependencyCycle Model { get; set; }
        }

        public class ALinkedSource : ILinkedSource<A>
        {
            public B B { get; set; }
            public A Model { get; set; }
        }

        public class BLinkedSource : ILinkedSource<B>
        {
            public C C { get; set; }
            public B Model { get; set; }
        }

        public class CLinkedSource : ILinkedSource<C>
        {
            public A A { get; set; }
            public C Model { get; set; }
        }

        public class DependencyCycle
        {
            public string Id { get; set; }
            public string AId { get; set; }
            public string BId { get; set; }
            public string CId { get; set; }
        }

        public class A
        {
            public string Id { get; set; }
            public string BId { get; set; }
        }

        public class B
        {
            public string Id { get; set; }
            public string CId { get; set; }
        }

        public class C
        {
            public string Id { get; set; }
            public string AId { get; set; }
        }
    }
}