using System.Collections.Generic;
using System.Linq;
using LinkIt.LinkTargets;
using LinkIt.PublicApi;
using LinkIt.Tests.Shared;
using NUnit.Framework;

namespace LinkIt.Tests.LinkTargets {
    [TestFixture]
    public class SingleValueLinkTargetTests {
        private LinkedSource _actual;
        private SingleValueLinkTarget<LinkedSource, Image> _sut;

        [SetUp]
        public void SetUp() {
            _actual = new LinkedSource();

            _sut = (SingleValueLinkTarget<LinkedSource, Image>)LinkTargetFactory.Create<LinkedSource, Image>(
                linkedSource => linkedSource.Image
            );
        }

        [Test]
        public void SetTargetProperty() {
            _sut.LazyInit(_actual,1);
            _sut.SetLinkTargetValue(_actual, new Image { Alt = "the-alt" }, 0);

            Assert.That(_actual.Image.Alt, Is.EqualTo("the-alt"));
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public Image Image { get; set; }
        }

        public class Model {
            public int Id { get; set; }
            public string ImageId { get; set; }
        }

    }
}
