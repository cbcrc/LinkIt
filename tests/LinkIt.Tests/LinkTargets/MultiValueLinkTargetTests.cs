// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using LinkIt.LinkTargets;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.LinkTargets
{
    public class MultiValueLinkTargetTests
    {
        [Fact]
        public void SetListTargetProperty()
        {
            var actual = new LinkedSourceWithList();
            var sut = (MultiValueLinkTarget<LinkedSourceWithList, Image>) LinkTargetFactory.Create<LinkedSourceWithList, Image>(
                linkedSource => linkedSource.Images);

            sut.LazyInit(actual, 2);
            sut.SetLinkTargetValue(actual, new Image { Alt = "the-alt1" }, 0);
            sut.SetLinkTargetValue(actual, new Image { Alt = "the-alt2" }, 1);

            Assert.Equal("the-alt1", actual.Images[0].Alt);
            Assert.Equal("the-alt2", actual.Images[1].Alt);
        }

        [Fact]
        public void SetArrayTargetProperty()
        {
            var actual = new LinkedSourceWithArray();
            var sut = (MultiValueLinkTarget<LinkedSourceWithArray, Image>) LinkTargetFactory.Create<LinkedSourceWithArray, Image>(
                linkedSource => linkedSource.Images);

            sut.LazyInit(actual, 2);
            sut.SetLinkTargetValue(actual, new Image { Alt = "the-alt1" }, 0);
            sut.SetLinkTargetValue(actual, new Image { Alt = "the-alt2" }, 1);

            Assert.Equal("the-alt1", actual.Images[0].Alt);
            Assert.Equal("the-alt2", actual.Images[1].Alt);
        }

        [Fact]
        public void SetIListTargetProperty()
        {
            var actual = new LinkedSourceWithIList();
            var sut = (MultiValueLinkTarget<LinkedSourceWithIList, Image>) LinkTargetFactory.Create<LinkedSourceWithIList, Image>(
                linkedSource => linkedSource.Images);

            sut.LazyInit(actual, 2);
            sut.SetLinkTargetValue(actual, new Image { Alt = "the-alt1" }, 0);
            sut.SetLinkTargetValue(actual, new Image { Alt = "the-alt2" }, 1);

            Assert.Equal("the-alt1", actual.Images[0].Alt);
            Assert.Equal("the-alt2", actual.Images[1].Alt);
        }

        public class LinkedSourceWithList : ILinkedSource<Model>
        {
            public List<Image> Images { get; set; }
            public Model Model { get; set; }
        }

        public class LinkedSourceWithArray : ILinkedSource<Model>
        {
            public Image[] Images { get; set; }
            public Model Model { get; set; }
        }

        public class LinkedSourceWithIList : ILinkedSource<Model>
        {
            public IList<Image> Images { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public List<string> ImageIds { get; set; }
        }
    }
}