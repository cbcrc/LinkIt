using System.Linq;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Core
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkConfig_ByIdsTests
    {
        private ReferenceLoaderStub _referenceLoaderStub;
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource=>linkedSource.Model.SummaryImageId,
                    linkedSource=>linkedSource.SummaryImage
                );

            _referenceLoaderStub = new ReferenceLoaderStub();
            _sut = loadLinkProtocolBuilder.Build(()=>_referenceLoaderStub);
        }


        [Test]
        public void LoadLinkById() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById("one");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLinkById_WithNullId_ShouldLinkNull() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById<string>(null);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void LoadLinkById_WithNullId_ShouldLinkNullWithoutLoading() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById<string>(null);

            var loadedReferenceTypes = _referenceLoaderStub.RecordedLookupIdContexts
                .First()
                .GetReferenceTypes();

            Assert.That(loadedReferenceTypes, Is.Empty);
        }

        [Test]
        public void LoadLinkById_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById("cannot-be-resolved");

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void LoadLinkByIds() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds("one","two");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLinkByIds_WithNullInReferenceIds_ShouldLinkNull() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds("one", null, "two");
            
            Assert.That(actual.Count, Is.EqualTo(3));
            Assert.That(actual[1], Is.Null);
        }

        [Test]
        public void LoadLinkByIds_WithListOfNulls_ShouldLinkNullWithoutLoading() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds<string>(null, null);

            Assert.That(actual, Is.EquivalentTo(new string[]{null,null}));

            var loadedReferenceTypes = _referenceLoaderStub.RecordedLookupIdContexts
                .First()
                .GetReferenceTypes();

            Assert.That(loadedReferenceTypes, Is.Empty);
        }


        [Test]
        public void LoadLinkByIds_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySet(){
            string[] modelIds = null;
            TestDelegate act = () => _sut.LoadLink<PersonLinkedSource>().ByIds(modelIds);

            Assert.That(act,
                Throws.ArgumentException
                    .With.Message.Contains("null array").And
                    .With.Message.Contains("modelIds")
            );
        }

        [Test]
        public void LoadLinkByIds_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds("a", "a");

            var linkedSourceModelIds = actual.Select(linkedSource => linkedSource.Model.Id);
            Assert.That(linkedSourceModelIds, Is.EquivalentTo(new[] { "a", "a" }));

            var loadedPersonIds = _referenceLoaderStub.RecordedLookupIdContexts
                .First()
                .GetReferenceIds<Person, string>();
            
            Assert.That(loadedPersonIds, Is.EquivalentTo(new[] { "a" }));
        }


        [Test]
        public void LoadLinkByIds_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds("cannot-be-resolved");

            Assert.That(actual.Single(), Is.Null);
        }
    }

}