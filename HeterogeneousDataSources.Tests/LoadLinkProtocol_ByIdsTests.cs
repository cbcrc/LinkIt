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
        public void LoadLinkByIds_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySet() {
            var actual = _sut.LoadLink<LinkedSource>().ByIds<string>(null);

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void LoadLinkByIds_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            var actual = _sut.LoadLink<LinkedSource>().ByIds("a", "a");

            var linkedSourceModelIds = actual.Select(linkedSource => linkedSource.Model.Id);
            Assert.That(linkedSourceModelIds, Is.EquivalentTo(new[] { "a", "a" }));
        }


        [Test]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<LinkedSource>().ByIds("cannot-be-resolved");

            Assert.That(actual.Single(), Is.Null);
        }

        public class LinkedSource : ILinkedSource<Image> {
            public Image Model { get; set; }
        }
    }

}