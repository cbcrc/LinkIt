using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class NestedPolymorphicReferenceTests {
        private FakeReferenceLoader2<WithNestedPolymorphicReference, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithNestedPolymorphicReferenceLinkedSource>()
                .IsRoot<string>()
                .LoadLinkNestedLinkedSource(
                    linkedSource => linkedSource.Model.PolyIds,
                    linkedSource => linkedSource.Contents,
                    reference => reference.GetType(),
                    includes => includes
                        .When<PersonLinkedSource, string>(
                            typeof(string),
                            reference => (string)reference)
                        .When<PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource, string>(
                            typeof(int),
                            reference => ((int)reference).ToString()));

            _fakeReferenceLoader =
                new FakeReferenceLoader2<WithNestedPolymorphicReference, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_NestedPolymorphicReference() {
            _fakeReferenceLoader.FixValue(
                new WithNestedPolymorphicReference {
                    Id = "1",
                    PolyIds = new List<object>{"p1",32}
                }
            );

            var actual = _sut.LoadLink<WithNestedPolymorphicReferenceLinkedSource>("1");

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
