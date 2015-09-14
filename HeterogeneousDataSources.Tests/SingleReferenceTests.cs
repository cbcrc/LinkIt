using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class SingleReferenceTests
    {
        private FakeReferenceLoader2<SingleReferenceContent, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .IsRoot<string>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _fakeReferenceLoader = 
                new FakeReferenceLoader2<SingleReferenceContent, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_SingleReference()
        {
            _fakeReferenceLoader.FixValue(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "a"
                }
            );

            var actual = _sut.LoadLink<SingleReferenceLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_SingleReferenceWithoutReferenceId_ShouldLinkNull() {
            _fakeReferenceLoader.FixValue(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = null
                }
            );

            var actual = _sut.LoadLink<SingleReferenceLinkedSource>("1");

            Assert.That(actual.SummaryImage, Is.Null);
        }

        [Test]
        public void LoadLink_SingleReferenceCannotBeResolved_ShouldLinkNull() {
            _fakeReferenceLoader.FixValue(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "cannot-be-resolved"
                }
            );

            var actual = _sut.LoadLink<SingleReferenceLinkedSource>("1");

            Assert.That(actual.SummaryImage, Is.Null);
        }
    }

    public class SingleReferenceLinkedSource: ILinkedSource<SingleReferenceContent>
    {
        //stle: two steps loading sucks!
        public SingleReferenceContent Model { get; set; }
        public Image SummaryImage{ get; set; }
    }

    public class SingleReferenceContent {
        public string Id { get; set; }
        public string SummaryImageId { get; set; }
    }
}
