using System.Collections.Generic;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.Protocols;
using LinkIt.Tests.Polymorphic;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Exploratory {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class NestedPolymorphicReferenceTests {
        private LoadLinkConfig _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithNestedPolymorphicReferenceLinkedSource>()
                .PolymorphicLoadLinkForList(
                    linkedSource => linkedSource.Model.PolyIds,
                    linkedSource => linkedSource.Contents,
                    reference => reference.GetType(),
                    includes => includes
                        .Include<PersonLinkedSource>().AsNestedLinkedSourceById(
                            typeof(string),
                            reference => (string)reference)
                        .Include<PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            typeof(int),
                            reference => ((int)reference).ToString()));

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_NestedPolymorphicReference() {
            var actual = _sut.LoadLink<WithNestedPolymorphicReferenceLinkedSource>().FromModel(
                new WithNestedPolymorphicReference {
                    Id = "1",
                    PolyIds = new List<object> { "p1", 32 }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class WithNestedPolymorphicReferenceLinkedSource : ILinkedSource<WithNestedPolymorphicReference> {
            public WithNestedPolymorphicReference Model { get; set; }
            public List<object> Contents { get; set; }
        }

        public class WithNestedPolymorphicReference {
            public string Id { get; set; }
            public List<object> PolyIds { get; set; }
        }

        
    }
}
