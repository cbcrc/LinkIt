// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using LinkIt.LinkTargets;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.LinkTargets
{
    public class LinkTargetFactoryTests
    {

        [Fact]
        public void Create_LinkTargetShouldBeEquatable()
        {
            var summaryImage1 = LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.SummaryImage);
            var summaryImage2 = LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.SummaryImage);
            var anotherImage = LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.AnotherImage);

            Assert.True(summaryImage2.Equals(summaryImage1));
            Assert.False(anotherImage.Equals(summaryImage1));
        }


        [Fact]
        public void Create_WithNestedGetter_ShouldThrow()
        {
            Action act = () => LinkTargetFactory.Create<ForLinkedTargetLinkedSource, string>(
                linkedSource => linkedSource.SummaryImage.Alt);

            var exception = Assert.Throws<ArgumentException>(act);
            Assert.Contains("ForLinkedTargetLinkedSource", exception.Message);
            Assert.Contains("direct getter", exception.Message);
        }

        [Fact]
        public void Test_WithExpression_ShouldThrow()
        {
            Action act = () => LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.SummaryImage ?? new Image()
            );

            var exception = Assert.Throws<ArgumentException>(act);
            Assert.Contains("ForLinkedTargetLinkedSource", exception.Message);
            Assert.Contains("direct getter", exception.Message);
        }

        [Fact]
        public void Test_WithFunc_ShouldThrow()
        {
            Action act = () => LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.AnImageFunc()
            );

            var exception = Assert.Throws<ArgumentException>(act);
            Assert.Contains("ForLinkedTargetLinkedSource", exception.Message);
            Assert.Contains("direct getter", exception.Message);
        }

        [Fact]
        public void Test_WithPrivateSetter_ShouldThrow()
        {
            Action act = () => LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.APrivateSetterImage);

            var exception = Assert.Throws<ArgumentException>(act);
            Assert.Contains("ForLinkedTargetLinkedSource/APrivateSetterImage", exception.Message);
            Assert.Contains("read-write", exception.Message);
        }

        [Fact]
        public void Test_WithReadOnlyProperty_ShouldThrow()
        {
            Action act = () => LinkTargetFactory.Create<ForLinkedTargetLinkedSource, Image>(
                linkedSource => linkedSource.AReadOnlyImage);

            var exception = Assert.Throws<ArgumentException>(act);
            Assert.Contains("ForLinkedTargetLinkedSource/AReadOnlyImage", exception.Message);
            Assert.Contains("read-write", exception.Message);
        }
        public class ForLinkedTargetLinkedSource : ILinkedSource<Person>
        {
            public Image SummaryImage { get; set; }
            public Image AnotherImage { get; set; }
            public Image AReadOnlyImage => null;
            public Image APrivateSetterImage { get; private set; }
            public Person Model { get; set; }

            public Image AnImageFunc()
            {
                return null;
            }
        }
    }
}