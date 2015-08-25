using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests {
    [TestFixture]
    public class LookupIdContextTests {
        private LookupIdContext _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new LookupIdContext();
        }

        [Test]
        public void Add_Distinct_ShouldAdd()
        {
            _sut.Add<Image,string>(new List<string> { "a", "b" });

            Assert.That(_sut.GetReferenceIds<Image, string>(), Is.EquivalentTo(new []{"a","b"}));
        }

        [Test]
        public void Add_WithNull_NullShouldNotBeAdded() {
            _sut.Add<Image, string>(new List<string> { "a", null, "b" });

            Assert.That(_sut.GetReferenceIds<Image, string>(), Is.EquivalentTo(new[] { "a", "b" }));
        }

        [Test]
        public void Add_WithDuplicates_DuplicatesShouldNotBeAdded() {
            _sut.Add<Image,string>(new List<string>{ "a", "a", "b" });

            Assert.That(_sut.GetReferenceIds<Image, string>(), Is.EquivalentTo(new[] { "a", "b" }));
        }

        [Test]
        public void Add_SameReferenceTypeTwice_ShouldThrow() {
            _sut.Add<Image, string>(new List<string> {});

            TestDelegate act =
                () => _sut.Add<Image, string>(new List<string> {});

            Assert.That(
                act, 
                Throws.InstanceOf<InvalidOperationException>()
            );
        }

    }
}
