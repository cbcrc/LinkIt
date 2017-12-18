#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class LoadLinkModelTests
    {
        private ILoadLinkProtocol _sut;

        public LoadLinkModelTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SingleReferenceLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);
            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async Task LoadLink_WithModel_ShouldLinkModel()
        {
            var actual = await _sut.LoadLink<SingleReferenceLinkedSource>().FromModelAsync(
                new SingleReferenceContent
                {
                    Id = "1",
                    SummaryImageId = "a"
                }
            );

            Assert.Equal("1", actual.Model.Id);
            Assert.Equal("a", actual.SummaryImage.Id);
        }

        [Fact]
        public async Task LoadLink_WithWrontModelType_ShouldThrow()
        {
            Func<Task> act = async () =>
                await _sut.LoadLink<SingleReferenceLinkedSource>()
                    .FromModelAsync(
                        "The model of SingleReferenceLinkedSource is not a string"
                    );

            var ex = await Assert.ThrowsAsync<ArgumentException>(act);
            Assert.Contains("SingleReferenceContent", ex.Message);
            Assert.Contains("String", ex.Message);
        }

        [Fact]
        public async Task LoadLink_ModelWithNullModel_ShouldReturnNull()
        {
            var actual = await _sut.LoadLink<SingleReferenceLinkedSource>().FromModelAsync<SingleReferenceContent>(null);

            Assert.Null(actual);
        }

        [Fact]
        public async Task LoadLink_WithModels_ShouldLinkModels()
        {
            var actual = await _sut.LoadLink<SingleReferenceLinkedSource>().FromModelsAsync(
                new List<SingleReferenceContent>
                {
                    new SingleReferenceContent
                    {
                        Id = "1",
                        SummaryImageId = "a"
                    },
                    new SingleReferenceContent
                    {
                        Id = "2",
                        SummaryImageId = "b"
                    }
                }
            );

            var summaryImageIds = actual
                .Select(item => item.SummaryImage.Id)
                .ToList();
            Assert.Equal(new[] { "a", "b" }, summaryImageIds);
        }

        [Fact]
        public async Task LoadLink_ModelsWithNullModel_ShouldLinkNull()
        {
            var actual = await _sut.LoadLink<SingleReferenceLinkedSource>().FromModelsAsync(new List<SingleReferenceContent> { null });

            Assert.Empty(actual);
        }

        [Fact]
        public async Task LoadLink_ModelsWithWrongModelType_ShouldThrow()
        {
            Func<Task> act = async () => await _sut.LoadLink<SingleReferenceLinkedSource>().FromModelsAsync(
                new List<string>
                {
                    "The model of SingleReferenceLinkedSource is not a string"
                }
            );

            var ex = await Assert.ThrowsAsync<ArgumentException>(act);
            Assert.Contains("SingleReferenceContent", ex.Message);
            Assert.Contains("String", ex.Message);
        }
    }
}