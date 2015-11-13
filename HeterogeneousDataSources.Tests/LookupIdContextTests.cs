using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.Tests.Shared;
using LinkIt.Protocols;
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
            _sut.AddSingle<Image,string>("a");
            _sut.AddSingle<Image, string>("b");

            Assert.That(_sut.GetReferenceIds<Image, string>(), Is.EquivalentTo(new []{"a","b"}));
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

            var actual = _sut.GetReferenceTypes();

            Assert.That(actual, Is.Empty);
        }

    }
}
