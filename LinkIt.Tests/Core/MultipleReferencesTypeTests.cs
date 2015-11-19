using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Core {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class MultipleReferencesTypeTests
    {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<MultipleReferencesTypeLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                )
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.AuthorId,
                    linkedSource => linkedSource.Author
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_MultipleReferencesTypeTests()
        {
            var actual = _sut.LoadLink<MultipleReferencesTypeLinkedSource>().FromModel(
                new MultipleReferencesTypeContent() {
                    Id = 1,
                    SummaryImageId = "a",
                    AuthorId = "32"
                }
            );

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
