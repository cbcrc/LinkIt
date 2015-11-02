using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkProtocol_ByIdsTests
    {
        private FakeReferenceLoader<Image, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            _fakeReferenceLoader =
                new FakeReferenceLoader<Image, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        //stle: test: more efficient way to test all cases
        //stle: test: ensure by models is also covered
        [Test]
        public void LoadLinkByIds() {
            var actual = _sut.LoadLink<LinkedSource>().ByIds("one","two");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLinkByIds_WithNullInReferenceIds_ShouldLinkNull() {
            var actual = _sut.LoadLink<LinkedSource>().ByIds("one", null, "two");
            
            Assert.That(actual.Count, Is.EqualTo(3));
            Assert.That(actual[1], Is.Null);
        }

        [Test]
        public void LoadLinkByIds_WithListOfNulls_ShouldLinkNullWithoutLoading() {
            var actual = _sut.LoadLink<LinkedSource>().ByIds<string>(null, null);

            Assert.That(actual, Is.EquivalentTo(new string[]{null,null}));

            var loadedReferenceTypes = _fakeReferenceLoader.RecordedLookupIdContexts
                .Single()
                .GetReferenceTypes();

            Assert.That(loadedReferenceTypes, Is.Empty);
        }


        [Test]
        public void LoadLinkByIds_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySet(){
            string[] modelIds = null;
            TestDelegate act = () => _sut.LoadLink<LinkedSource>().ByIds(modelIds);

            Assert.That(act,
                Throws.ArgumentException
                    .With.Message.Contains("null array").And
                    .With.Message.Contains("modelIds")
            );
        }

        [Test]
        public void LoadLinkByIds_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            var actual = _sut.LoadLink<LinkedSource>().ByIds("a", "a");

            var linkedSourceModelIds = actual.Select(linkedSource => linkedSource.Model.Id);
            Assert.That(linkedSourceModelIds, Is.EquivalentTo(new[] { "a", "a" }));

            var loadedImageIds = _fakeReferenceLoader.RecordedLookupIdContexts
                .Single()
                .GetReferenceIds<Image, string>();
            
            Assert.That(loadedImageIds, Is.EquivalentTo(new[] { "a" }));
        }


        [Test]
        public void LoadLinkByIds_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<LinkedSource>().ByIds("cannot-be-resolved");

            Assert.That(actual.Single(), Is.Null);
        }

        public class LinkedSource : ILinkedSource<Image> {
            public Image Model { get; set; }
        }
    }

}