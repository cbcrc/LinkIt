using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class OptionalNestedLinkedSourceTests
    {
        private FakeReferenceLoader<Model, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkNestedLinkedSource(
                    linkedSource => linkedSource.Model.MediaId,
                    linkedSource => linkedSource.Media
                );

            loadLinkProtocolBuilder.For<MediaLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<Model, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_WithValue_ShouldLinkMedia() {
            _fakeReferenceLoader.FixValue(
                new Model {
                    Id = "1",
                    MediaId = 32
                }
            );

            var actual = _sut.LoadLink<LinkedSource>().ById("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_WithoutValue_ShouldLinkNull() {
            _fakeReferenceLoader.FixValue(
                new Model {
                    Id = "1",
                    MediaId = null
                }
            );

            var actual = _sut.LoadLink<LinkedSource>().ById("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public MediaLinkedSource Media { get; set; }
        }

        public class MediaLinkedSource : ILinkedSource<Media> {
            public Media Model { get; set; }
            public Image SummaryImage { get; set; }
        }

        public class Model {
            public string Id { get; set; }
            public int? MediaId { get; set; }
        }
    }
}