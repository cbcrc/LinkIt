#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.Samples.LinkedSources;
using Xunit;

namespace LinkIt.Samples
{
    public class GettingStarted: IClassFixture<LoadLinkProtocolFixture>
    {
        private readonly LoadLinkProtocolFixture _fixture;

        public GettingStarted(LoadLinkProtocolFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task LoadLink_ById()
        {
            var actual = await _fixture.LoadLinkProtocol.LoadLink<MediaLinkedSource>().ByIdAsync(1);

            Assert.Equal(1, actual.Model.Id);
            Assert.Collection(
                actual.Tags,
                t => Assert.Equal(1001, t.Id),
                t => Assert.Equal(1002, t.Id)
            );
        }

        [Fact]
        public async Task LoadLink_ByIds()
        {
            var actual = (await _fixture.LoadLinkProtocol.LoadLink<MediaLinkedSource>().ByIdsAsync(new List<int> { 1, 2, 3 }))
                .OrderBy(x => x.Model.Id)
                .ToList();

            Assert.Equal(3, actual.Count);

            Assert.Equal(1, actual[0].Model.Id);
            Assert.Collection(
                actual[0].Tags,
                t => Assert.Equal(1001, t.Id),
                t => Assert.Equal(1002, t.Id)
            );

            Assert.Equal(2, actual[1].Model.Id);
            Assert.Collection(
                actual[1].Tags,
                t => Assert.Equal(1002, t.Id),
                t => Assert.Equal(1003, t.Id)
            );

            Assert.Equal(3, actual[2].Model.Id);
            Assert.Collection(
                actual[2].Tags,
                t => Assert.Equal(1003, t.Id),
                t => Assert.Equal(1004, t.Id)
            );
        }
    }
}