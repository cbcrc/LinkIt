using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicSubLinkedSourcesTests {
        private FakeReferenceLoader<WithPolymorphicSubLinkedSourceContent, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        //[SetUp]
        //public void SetUp() {
        //    var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
        //    loadLinkProtocolBuilder.For<WithPolymorphicSubLinkedSource>()
        //        .IsRoot<string>()
        //        .LoadLinkNestedLinkedSource(
        //            linkedSource => linkedSource.Model.Subs,
        //            linkedSource => linkedSource.Subs,
        //            reference => reference.GetType(),
        //            includes => includes
        //                .When<SubContentWithImageLinkedSource, string>(
        //                    "person",
        //                    reference => reference.GetType().Name)
        //        //.WhenSub<SubContentWithImageLinkedSource>(
        //                //    typeof(SubContentWithImage)
        //                //)
        //                //.WhenSub<SubContentWithoutReferences>(
        //                //    typeof(SubContentWithoutReferences)
        //                //)
        //        );

        //    _fakeReferenceLoader =
        //        new FakeReferenceLoader<WithPolymorphicSubLinkedSourceContent, string>(reference => reference.Id);
        //    _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        //}



        //[Test]
        //public void LoadLink_SubContentWithoutReferences() {
        //    _fakeReferenceLoader.FixValue(
        //        new WithPolymorphicSubLinkedSourceContent {
        //            Id = "1",
        //            Subs = new List<IPolymorphicModel>
        //            {
        //                new SubContentWithImage
        //                {
        //                    Id="a",
        //                    ImageId = "i-a"
        //                },
        //                new SubContentWithoutReferences
        //                {
        //                    Id="b",
        //                    Title = "sub-b"
        //                },
        //            }
        //        }
        //    );

        //    var actual = _sut.LoadLink<WithPolymorphicSubLinkedSource>("1");

        //    ApprovalsExt.VerifyPublicProperties(actual);
        //}

        public class WithPolymorphicSubLinkedSource : ILinkedSource<WithPolymorphicSubLinkedSourceContent> {
            public WithPolymorphicSubLinkedSourceContent Model { get; set; }
            public List<IPolymorphicSource> Subs { get; set; }
        }

        public class SubContentWithImageLinkedSource : ILinkedSource<SubContentWithImage>, IPolymorphicSource {
            public SubContentWithImage Model { get; set; }
            public Image Image { get; set; }
        }

        public class SubContentWithoutReferencesLinkedSource : ILinkedSource<SubContentWithoutReferences>, IPolymorphicSource{
            public SubContentWithoutReferences Model { get; set; }
        }

        public class WithPolymorphicSubLinkedSourceContent {
            public string Id { get; set; }
            public List<IPolymorphicModel> Subs { get; set; }
        }

        public class SubContentWithImage : IPolymorphicModel {
            public string Id { get; set; }
            public string ImageId{ get; set; }
        }

        public class SubContentWithoutReferences : IPolymorphicModel {
            public string Id { get; set; }
            public string Title { get; set; }
        }
    }
}
