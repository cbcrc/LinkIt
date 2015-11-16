using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.Protocols;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class NestedLinkedSourceTests
    {
        private LoadLinkConfig _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<NestedLinkedSource>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.AuthorDetailId,
                    linkedSource => linkedSource.AuthorDetail)
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ClientSummaryId,
                    linkedSource => linkedSource.ClientSummary);

            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_NestedLinkedSource()
        {
            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "32",
                    ClientSummaryId = "33"
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_DifferendKindOfPersonInSameRootLinkedSource_ShouldNotLoadImageFromClientSummary() {
            _sut.LoadLink<NestedLinkedSource>().FromModel(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "32",
                    ClientSummaryId = "666" //Image repository throws an exception for "person-img-666" 
                }
            );

            //stle: improve this by allowing test visibility on which image id was resolved
            //assert that does not throw
        }

        [Test]
        public void LoadLink_NestedLinkedSourceWithoutReferenceId_ShouldLinkNull() {
            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = null,
                    ClientSummaryId = "33"
                }
            );

            Assert.That(actual.AuthorDetail, Is.Null);
        }

        [Test]
        public void LoadLink_NestedLinkedSourceCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "cannot-be-resolved",
                    ClientSummaryId = "33"
                }
            );

            Assert.That(actual.AuthorDetail, Is.Null);
        }

        [Test]
        public void LoadLink_NestedLinkedSourceRootCannotBeResolved_ShouldReturnNullAsRoot() {
            NestedContent model = null;

            var actual = _sut.LoadLink<NestedLinkedSource>().FromModel(model);

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
