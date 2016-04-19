#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using LinkIt.LinkTargets;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;

namespace LinkIt.Tests.LinkTargets {
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
