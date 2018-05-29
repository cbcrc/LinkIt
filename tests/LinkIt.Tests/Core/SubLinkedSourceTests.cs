// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Threading.Tasks;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class SubLinkedSourceTests
    {
        private readonly ILoadLinkProtocol _sut;

        public SubLinkedSourceTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SubContentOwnerLinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubContent,
                    linkedSource => linkedSource.SubContent)
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubSubContent,
                    linkedSource => linkedSource.SubSubContent);
            loadLinkProtocolBuilder.For<SubContentLinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubSubContent,
                    linkedSource => linkedSource.SubSubContent);
            loadLinkProtocolBuilder.For<SubSubContentLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async Task LoadLink_SubLinkedSource()
        {
            var actual = await _sut.LoadLink<SubContentOwnerLinkedSource>().FromModelAsync(
                new SubContentOwner
                {
                    Id = "1",
                    SubContent = new SubContent
                    {
                        SubSubContent = new SubSubContent
                        {
                            SummaryImageId = "a"
                        }
                    },
                    SubSubContent = new SubSubContent
                    {
                        SummaryImageId = "b"
                    }
                }
            );

            Assert.Equal("a", actual.SubContent.SubSubContent.SummaryImage.Id);
            Assert.Equal("b", actual.SubSubContent.SummaryImage.Id);
        }

        [Fact]
        public async Task LoadLink_SingleReferenceWithoutReferenceId_ShouldLinkNull()
        {
            var actual = await _sut.LoadLink<SubContentOwnerLinkedSource>().FromModelAsync(
                new SubContentOwner
                {
                    Id = "1",
                    SubContent = new SubContent
                    {
                        SubSubContent = null
                    },
                    SubSubContent = null
                }
            );

            Assert.Null(actual.SubContent.SubSubContent);
            Assert.Null(actual.SubSubContent);
        }
    }

    public class SubContentOwnerLinkedSource : ILinkedSource<SubContentOwner>
    {
        public SubContentLinkedSource SubContent { get; set; }
        public SubSubContentLinkedSource SubSubContent { get; set; }
        public SubContentOwner Model { get; set; }
    }

    public class SubContentLinkedSource : ILinkedSource<SubContent>
    {
        public SubSubContentLinkedSource SubSubContent { get; set; }
        public SubContent Model { get; set; }
    }

    public class SubSubContentLinkedSource : ILinkedSource<SubSubContent>
    {
        public Image SummaryImage { get; set; }
        public SubSubContent Model { get; set; }
    }

    public class SubContentOwner
    {
        public string Id { get; set; }
        public SubContent SubContent { get; set; }
        public SubSubContent SubSubContent { get; set; }
    }

    public class SubContent
    {
        public SubSubContent SubSubContent { get; set; }
    }

    public class SubSubContent
    {
        public string SummaryImageId { get; set; }
    }
}