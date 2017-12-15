#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using LinkIt.LinkTargets;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.LinkTargets
{
    public class MultiValueLinkTargetTests
    {
        private readonly LinkedSource _actual;
        private readonly MultiValueLinkTarget<LinkedSource, Image> _sut;

        public MultiValueLinkTargetTests()
        {
            _actual = new LinkedSource();

            _sut = (MultiValueLinkTarget<LinkedSource, Image>) LinkTargetFactory.Create<LinkedSource, Image>(
                linkedSource => linkedSource.Images);
        }

        [Fact]
        public void SetTargetProperty()
        {
            _sut.LazyInit(_actual, 2);
            _sut.SetLinkTargetValue(_actual, new Image { Alt = "the-alt1" }, 0);
            _sut.SetLinkTargetValue(_actual, new Image { Alt = "the-alt2" }, 1);

            Assert.Equal("the-alt1", _actual.Images[0].Alt);
            Assert.Equal("the-alt2", _actual.Images[1].Alt);
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public List<Image> Images { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public List<string> ImageIds { get; set; }
        }
    }
}