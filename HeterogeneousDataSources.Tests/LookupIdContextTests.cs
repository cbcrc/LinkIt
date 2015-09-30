using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests {
    [TestFixture]
    public class LookupIdContextTests {
        private LookupIdContext _sut;

        [SetUp]
        public void SetUp() {
            _sut = new LookupIdContext();
        }

        [Test]
        public void Add_Distinct_ShouldAdd() {
            _sut.AddSingle<Image, string>("a");
            _sut.AddSingle<Image, string>("b");

            Assert.That(_sut.GetReferenceIds<Image, string>(), Is.EquivalentTo(new[] { "a", "b" }));
        }

        [Test]
        public void Add_WithDuplicates_DuplicatesShouldNotBeAdded() {
            _sut.AddSingle<Image, string>("a");
            _sut.AddSingle<Image, string>("a");
            _sut.AddSingle<Image, string>("b");

            Assert.That(_sut.GetReferenceIds<Image, string>(), Is.EquivalentTo(new[] { "a", "b" }));
        }

        [Test]
        public void Add_NullId_ShouldIgnoreNullId() {
            _sut.AddSingle<Image, string>(null);

            //stle: think of how we can
            //  avoid depending on reference loader to optimize for empty ids
            //  and
            //  to have a simple list initilization mechanism in linking (especially in poly)
            var actual = _sut.GetReferenceIds<Image, string>();

            Assert.That(actual, Is.Empty);


            //stle: think of 
            //Assert.That(
            //    _sut.GetReferenceTypes().Any(referenceType=>referenceType==typeof(Image)), 
            //    Is.False
            //);
            //Assert.That(act, Throws.InvalidOperationException.With.Message.ContainsSubstring("Image"));
        }

    }
}
