﻿using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.Protocols;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class PolymorphicNestedLinkedSourceTests {
        private FakeReferenceLoader<WithNestedPolymorphicContent, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithNestedPolymorphicContentLinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.ContentContextualization,
                    linkedSource => linkedSource.Content,
                    link => link.ContentType,
                    includes => includes
                        .Include<PolymorphicNestedLinkedSourcesTests.PersonWithoutContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            "person",
                            link => (string)link.Id)
                        .Include<PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            "image",
                            link => (string)link.Id,
                            (linkedSource, referenceIndex, childLinkedSource) =>
                                childLinkedSource.ContentContextualization = linkedSource.Model.ContentContextualization)
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<WithNestedPolymorphicContent, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_NestedPolymorphicContent() {
            _fakeReferenceLoader.FixValue(
                new WithNestedPolymorphicContent {
                    Id = "1",
                    ContentContextualization = new PolymorphicNestedLinkedSourcesTests.ContentContextualization{
                        ContentType = "person",
                        Id = "p1",
                        Title = "altered person title"
                    }
                }
            );

            var actual = _sut.LoadLink<WithNestedPolymorphicContentLinkedSource>().ById("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_NestedPolymorphicContentWithContextualization_ShouldInitContextualization() {
            _fakeReferenceLoader.FixValue(
                new WithNestedPolymorphicContent {
                    Id = "1",
                    ContentContextualization = new PolymorphicNestedLinkedSourcesTests.ContentContextualization {
                        ContentType = "image",
                        Id = "i1",
                        Title = "altered image title"
                    }
                }
            );

            var actual = _sut.LoadLink<WithNestedPolymorphicContentLinkedSource>().ById("1");

            var contentAsImage =
                actual.Content as PolymorphicNestedLinkedSourcesTests.ImageWithContextualizationLinkedSource;
            Assert.That(contentAsImage.ContentContextualization.Title, Is.EqualTo("altered image title"));
        }


        public class WithNestedPolymorphicContentLinkedSource : ILinkedSource<WithNestedPolymorphicContent> {
            public WithNestedPolymorphicContent Model { get; set; }
            public IPolymorphicSource Content { get; set; }
        }

        public class WithNestedPolymorphicContent {
            public string Id { get; set; }
            public PolymorphicNestedLinkedSourcesTests.ContentContextualization ContentContextualization { get; set; }
        }
    }
}