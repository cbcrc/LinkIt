using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class OptionalNestedLinkedSourceTests
    {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<LinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.MediaId,
                    linkedSource => linkedSource.Media
                );

            loadLinkProtocolBuilder.For<MediaLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_WithValue_ShouldLinkMedia() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
                    Id = "1",
                    MediaId = 32
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_WithoutValue_ShouldLinkNull() {
            var actual = _sut.LoadLink<LinkedSource>().FromModel(
                new Model {
                    Id = "1",
                    MediaId = null
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public MediaLinkedSource Media { get; set; }
        }

        public class Model {
            public string Id { get; set; }
            public int? MediaId { get; set; }
        }
    }
}