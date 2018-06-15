// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Shared;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LoadLinkPrimitiveModelTests
    {
        private readonly ILoadLinkProtocol _sut;

        public LoadLinkPrimitiveModelTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PrimitiveLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model,
                    linkedSource => linkedSource.Media);
            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async Task LoadLink_WithModel_ShouldLinkModel()
        {
            var actual = await _sut.LoadLink<PrimitiveLinkedSource>().FromModelAsync(1);

            Assert.Equal(1, actual.Model);
            Assert.Equal(1, actual.Media.Id);
        }

        [Fact]
        public async Task LoadLink_WithModels_ShouldLinkModels()
        {
            var actual = await _sut.LoadLink<PrimitiveLinkedSource>().FromModelsAsync(new List<int> { 1, 2 });

            var ids = actual
                .Select(item => item.Media.Id)
                .ToList();
            Assert.Equal(new[] { 1, 2 }, ids);
        }

        [Fact]
        public async Task LoadLink_ModelsWithWrongModelType_ShouldThrow()
        {
            Func<Task> act = async () => await _sut.LoadLink<PrimitiveLinkedSource>().FromModelsAsync(
                new List<string>
                {
                    "The model of PrimitiveLinkedSource is not a string"
                }
            );

            var ex = await Assert.ThrowsAsync<LinkItException>(act);
            Assert.Contains("int", ex.Message);
            Assert.Contains("string", ex.Message);
        }
    }

    public class PrimitiveLinkedSource : ILinkedSource<int>
    {
        public int Model { get; set; }
        public Media Media { get; set; }

    }
}