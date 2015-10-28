using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class MultipleReferencesTypeTests
    {
        private FakeReferenceLoader<MultipleReferencesTypeContent, int> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<MultipleReferencesTypeLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                )
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.AuthorId,
                    linkedSource => linkedSource.Author
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<MultipleReferencesTypeContent, int>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_MultipleReferencesTypeTests()
        {
            _fakeReferenceLoader.FixValue(
                new MultipleReferencesTypeContent()
                {
                    Id = 1,
                    SummaryImageId = "a",
                    AuthorId = "32"
                }
            );

            var actual = _sut.LoadLink<MultipleReferencesTypeLinkedSource>().ById(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }


    public class MultipleReferencesTypeLinkedSource : ILinkedSource<MultipleReferencesTypeContent>
    {
        public MultipleReferencesTypeContent Model { get; set; }
        public Image SummaryImage { get; set; }
        public Person Author { get; set; }
    }

    public class MultipleReferencesTypeContent {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
        public string AuthorId { get; set; }
    }
}
