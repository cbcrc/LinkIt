using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests {
    [TestFixture]
    public class LinkTargetFactoryTests {
        [Test]
        public void Create_LinkTargetShouldBeEqualtable() {
            var summaryImage1 = LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.SummaryImage
            );
            var summaryImage2 = LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.SummaryImage
            );
            var anotherImage = LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.AnotherImage
            );

            Assert.That(summaryImage1.Equals(summaryImage2), Is.True);
            Assert.That(summaryImage1.Equals(anotherImage), Is.False);
        }


        [Test]
        public void Create_WithNestedGetter_ShouldThrow()
        {
            TestDelegate act = () => LinkTargetFactory.Create<ForLinkedTargetLinkedSource, string>(
                linkedSource => linkedSource.SummaryImage.Alt
            );

            Assert.That(act, Throws.ArgumentException
                .With.Message.ContainsSubstring("ForLinkedTargetLinkedSource").And
                .With.Message.ContainsSubstring("direct getter")
            );
        }

        [Test]
        public void Test_WithExpression_ShouldThrow() {
            TestDelegate act = () => LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.SummaryImage ?? new Image()
            );

            Assert.That(act, Throws.ArgumentException
                .With.Message.ContainsSubstring("ForLinkedTargetLinkedSource").And
                .With.Message.ContainsSubstring("direct getter")
            );
        }

        [Test]
        public void Test_WithFunc_ShouldThrow() {
            TestDelegate act = () => LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.AnImageFunc()
            );

            Assert.That(act, Throws.ArgumentException
                .With.Message.ContainsSubstring("ForLinkedTargetLinkedSource").And
                .With.Message.ContainsSubstring("direct getter")
            );
        }

        [Test]
        public void Test_WithReadOnlyProperty_ShouldThrow() {
            TestDelegate act = () => LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.AReadOnlyImage
            );

            Assert.That(act, Throws.ArgumentException
                .With.Message.ContainsSubstring("ForLinkedTargetLinkedSource/AReadOnlyImage").And
                .With.Message.ContainsSubstring("read-write")
            );
        }

        [Test]
        public void Test_WithPrivateSetter_ShouldThrow() {
            TestDelegate act = () => LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.APrivateSetterImage
            );

            Assert.That(act, Throws.ArgumentException
                .With.Message.ContainsSubstring("ForLinkedTargetLinkedSource/APrivateSetterImage").And
                .With.Message.ContainsSubstring("read-write")
            );
        }

        public class ForLinkedTargetLinkedSource : ILinkedSource<Person> {
            public Person Model { get; set; }
            public Image SummaryImage { get; set; }
            public Image AnotherImage { get; set; }
            
            public Image AnImageFunc(){ return null; }
            public Image AReadOnlyImage { get { return null; } }
            public Image APrivateSetterImage { get; private set; }
        }
    }
}
