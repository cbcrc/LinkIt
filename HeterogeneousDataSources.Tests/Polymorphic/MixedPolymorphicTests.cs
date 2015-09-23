using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class MixedPolymorphicTests {
        private FakeReferenceLoader<WithMixedPolymorphicContentLinkedSource, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        //[SetUp]
        //public void SetUp() {
        //    var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
        //    loadLinkProtocolBuilder.For<WithMixedPolymorphicContentLinkedSource>()
        //        .IsRoot<string>()
        //        .LoadLinkNestedLinkedSource(
        //            linkedSource => linkedSource.Model.ContentContextualization,
        //            linkedSource => linkedSource.Content,
        //            reference => reference.ContentType,
        //            includes => includes
        //                .When<PolymorphicNestedLinkedSourcesTests.PersonWithoutContextualizationLinkedSource, string>(
        //                    "person",
        //                    reference => (string)reference.Id)
        //                .When<PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource, string>(
        //                    "image",
        //                    reference => (string)reference.Id,
        //                    (linkedSource, referenceIndex, childLinkedSource) =>
        //                        childLinkedSource.ContentContextualization = linkedSource.Model.ContentContextualization)
        //        );

        //    _fakeReferenceLoader =
        //        new FakeReferenceLoader<WithMixedPolymorphicContentLinkedSource, string>(reference => reference.Id);
        //    _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        //}

        //[Test]
        //public void LoadLink_NestedPolymorphicContent() {
        //    _fakeReferenceLoader.FixValue(
        //        new WithNestedPolymorphicContent {
        //            Id = "1",
        //            ContentContextualization = new PolymorphicNestedLinkedSourcesTests.ContentContextualization{
        //                ContentType = "person",
        //                Id = "p1",
        //                Title = "altered person title"
        //            }
        //        }
        //    );

        //    var actual = _sut.LoadLink<WithNestedPolymorphicContentLinkedSource>("1");

        //    ApprovalsExt.VerifyPublicProperties(actual);
        //}


        public class WithMixedPolymorphicContentLinkedSource : ILinkedSource<WithMixedPolymorphicContent> {
            public WithMixedPolymorphicContent Model { get; set; }
            public List<object> MixedObject;
        }

        public class WithMixedPolymorphicContent {
            public string Id { get; set; }
            public List<MixedPolymorphicReference> MixedObjectIds;
        }

        public class MixedPolymorphicReference
        {
            public string Type { get; set; }
            public string Id { get; set; }
        }


    }
}
