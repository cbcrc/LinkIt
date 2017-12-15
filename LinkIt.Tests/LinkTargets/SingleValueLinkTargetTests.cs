#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using LinkIt.LinkTargets;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.LinkTargets
{
    public class SingleValueLinkTargetTests
    {
        public SingleValueLinkTargetTests()
        {
            _actual = new LinkedSource();

            _sut = (SingleValueLinkTarget<LinkedSource, Image>) LinkTargetFactory.Create<LinkedSource, Image>(
                linkedSource => linkedSource.Image);
        }

        private readonly LinkedSource _actual;
        private readonly SingleValueLinkTarget<LinkedSource, Image> _sut;

        [Fact]
        public void SetTargetProperty()
        {
            _sut.LazyInit(_actual, 1);
            _sut.SetLinkTargetValue(_actual, new Image { Alt = "the-alt" }, 0);

            Assert.Equal("the-alt", _actual.Image.Alt);
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public Image Image { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public string ImageId { get; set; }
        }
    }
}