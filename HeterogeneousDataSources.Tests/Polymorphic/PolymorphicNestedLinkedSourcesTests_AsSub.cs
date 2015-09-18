using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicNestedLinkedSourcesTests_AsSub {
        private FakeReferenceLoader<PolymorphicNestedLinkedSourcesTests.WithNestedPolymorphicContents, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PolymorphicNestedLinkedSourcesTests.WithNestedPolymorphicContentsLinkedSource>()
                .IsRoot<string>()
                .LoadLinkNestedLinkedSource(
                    linkedSource => linkedSource.Model.ContentContextualizations,
                    linkedSource => linkedSource.Contents,
                    reference => reference.ContentType,
                    includes => includes
                        .WhenSub<ContentContextualizationLinkedSourceA>(
                            "person"
                        )
                        .WhenSub<ContentContextualizationLinkedSourceB>(
                            "image"
                        )
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<PolymorphicNestedLinkedSourcesTests.WithNestedPolymorphicContents, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_NestedPolymorphicContents() {
            _fakeReferenceLoader.FixValue(
                new PolymorphicNestedLinkedSourcesTests.WithNestedPolymorphicContents {
                    Id = "1",
                    ContentContextualizations = new List<PolymorphicNestedLinkedSourcesTests.ContentContextualization>{
                        new PolymorphicNestedLinkedSourcesTests.ContentContextualization{
                            ContentType = "person",
                            Id = "p1",
                            Title = "altered person title"
                        },
                        new PolymorphicNestedLinkedSourcesTests.ContentContextualization{
                            ContentType = "image",
                            Id = "i1",
                            Title = "altered image title"
                        }
                    }
                }
            );

            var actual = _sut.LoadLink<PolymorphicNestedLinkedSourcesTests.WithNestedPolymorphicContentsLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class ContentContextualizationLinkedSourceA :IPolymorphicSource, ILinkedSource<PolymorphicNestedLinkedSourcesTests.ContentContextualization>
        {
            public PolymorphicNestedLinkedSourcesTests.ContentContextualization Model { get; set; }
            public string A { get; set; }
        }

        public class ContentContextualizationLinkedSourceB : IPolymorphicSource, ILinkedSource<PolymorphicNestedLinkedSourcesTests.ContentContextualization> {
            public PolymorphicNestedLinkedSourcesTests.ContentContextualization Model { get; set; }
            public string B { get; set; }
        }

    }
}
