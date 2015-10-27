using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class NestedLinkedSourceTests
    {
        private FakeReferenceLoader<NestedContent, int> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<NestedLinkedSource>()
                .LoadLinkNestedLinkedSource(
                    linkedSource => linkedSource.Model.AuthorDetailId,
                    linkedSource => linkedSource.AuthorDetail)
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.ClientSummaryId,
                    linkedSource => linkedSource.ClientSummary);

            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _fakeReferenceLoader =
                new FakeReferenceLoader<NestedContent, int>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_NestedLinkedSource()
        {
            _fakeReferenceLoader.FixValue(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "32",
                    ClientSummaryId = "33"
                }
            );

            var actual = _sut.LoadLink<NestedLinkedSource,int>(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_DifferendKindOfPersonInSameRootLinkedSource_ShouldNotLoadImageFromClientSummary() {
            _fakeReferenceLoader.FixValue(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "32",
                    ClientSummaryId = "666" //Image repository throws an exception for "person-img-666" 
                }
            );

            _sut.LoadLink<NestedLinkedSource,int>(1);

            //stle: improve this by allowing test visibility on which image id was resolved
            //assert that does not throw
        }

        [Test]
        public void LoadLink_NestedLinkedSourceWithoutReferenceId_ShouldLinkNull() {
            _fakeReferenceLoader.FixValue(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = null,
                    ClientSummaryId = "33"
                }
            );
            var actual = _sut.LoadLink<NestedLinkedSource,int>(1);

            Assert.That(actual.AuthorDetail, Is.Null);
        }

        [Test]
        public void LoadLink_NestedLinkedSourceCannotBeResolved_ShouldLinkNull() {
            _fakeReferenceLoader.FixValue(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "cannot-be-resolved",
                    ClientSummaryId = "33"
                }
            );

            var actual = _sut.LoadLink<NestedLinkedSource,int>(1);

            Assert.That(actual.AuthorDetail, Is.Null);
        }

        [Test]
        public void LoadLink_NestedLinkedSourceRootCannotBeResolved_ShouldReturnNullAsRoot() {
            _fakeReferenceLoader.FixValue(null);

            var actual = _sut.LoadLink<NestedLinkedSource,int>(1);

            Assert.That(actual, Is.Null);
        }
    }


    public class NestedLinkedSource:ILinkedSource<NestedContent>
    {
        public NestedContent Model { get; set; }
        public PersonLinkedSource AuthorDetail { get; set; }
        public Person ClientSummary { get; set; }
    }

    public class NestedContent {
        public int Id { get; set; }
        public string AuthorDetailId { get; set; }
        public string ClientSummaryId { get; set; }
    }

    public class PersonLinkedSource: ILinkedSource<Person>
    {
        public Person Model { get; set; }
        public Image SummaryImage{ get; set; }
    }
}
