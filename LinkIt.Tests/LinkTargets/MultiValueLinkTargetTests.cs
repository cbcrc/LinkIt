using System.Collections.Generic;
using System.Linq;
using LinkIt.LinkTargets;
using LinkIt.PublicApi;
using LinkIt.Tests.Shared;
using NUnit.Framework;

namespace LinkIt.Tests.LinkTargets {
    [TestFixture]
    public class MultiValueLinkTargetTests {
        private LinkedSource _actual;
        private MultiValueLinkTarget<LinkedSource, Image> _sut;

        [SetUp]
        public void SetUp() {
            _actual = new LinkedSource();

            _sut = (MultiValueLinkTarget<LinkedSource, Image>)LinkTargetFactory.Create<LinkedSource, Image>(
                linkedSource => linkedSource.Images
            );
        }

        [Test]
        public void SetTargetProperty() {
            _sut.LazyInit(_actual,2);
            _sut.SetLinkTargetValue(_actual, new Image { Alt = "the-alt1" }, 0);
            _sut.SetLinkTargetValue(_actual, new Image { Alt = "the-alt2" }, 1);

            Assert.That(_actual.Images[0].Alt, Is.EqualTo("the-alt1"));
            Assert.That(_actual.Images[1].Alt, Is.EqualTo("the-alt2"));
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public List<Image> Images { get; set; }
        }

        public class Model {
            public int Id { get; set; }
            public List<string> ImageIds { get; set; }
        }

    }
}
