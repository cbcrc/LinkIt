#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using LinkIt.Tests.Core;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class SubLinkedSourcesTests
    {
        private ILoadLinkProtocol _sut;

        public SubLinkedSourcesTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SubContentsOwnerLinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubContents,
                    linkedSource => linkedSource.SubContents)
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubSubContents,
                    linkedSource => linkedSource.SubSubContents);
            loadLinkProtocolBuilder.For<SubContentWithManySubSubContentsLinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubSubContents,
                    linkedSource => linkedSource.SubSubContents);
            loadLinkProtocolBuilder.For<SubSubContentLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }


        [Fact]
        public async Task LoadLink_SubLinkedSources()
        {
            var actual = await _sut.LoadLink<SubContentsOwnerLinkedSource>().FromModelAsync(
                new SubContentsOwner
                {
                    Id = "1",
                    SubContents = new List<SubContentWithManySubSubContents>
                    {
                        new SubContentWithManySubSubContents
                        {
                            SubSubContents = new List<SubSubContent>
                            {
                                new SubSubContent { SummaryImageId = "a" },
                                new SubSubContent { SummaryImageId = "b" }
                            }
                        }
                    },
                    SubSubContents = new List<SubSubContent>
                    {
                        new SubSubContent { SummaryImageId = "c" },
                        new SubSubContent { SummaryImageId = "d" }
                    }
                }
            );

            var linkedImagesIds = actual.SubContents.SelectMany(subContent => subContent.SubSubContents).Select(subSubContent => subSubContent.Model.SummaryImageId);
            Assert.Equal(new[] { "a", "b" }, linkedImagesIds);
            linkedImagesIds = actual.SubSubContents.Select(subSubContent => subSubContent.Model.SummaryImageId);
            Assert.Equal(new[] { "c", "d" }, linkedImagesIds);
        }

        [Fact]
        public async Task LoadLink_SubLinkedSourcesWithNullInReferenceIds_ShouldNotLinkNull()
        {
            var actual = await _sut.LoadLink<SubContentsOwnerLinkedSource>().FromModelAsync(
                new SubContentsOwner
                {
                    Id = "1",
                    SubContents = null, //dont-care
                    SubSubContents = new List<SubSubContent>{
                        new SubSubContent { SummaryImageId = "c" },
                        null,
                        new SubSubContent { SummaryImageId = "d" }
                    }
                 }
            );

            var linkedImagesIds = actual.SubSubContents.Select(subSubContent => subSubContent.Model.SummaryImageId);
            Assert.Equal(new[] { "c", "d" }, linkedImagesIds);
        }

        [Fact]
        public async Task LoadLink_SubLinkedSourcesWithoutReferenceIds_ShouldLinkEmptySet()
        {

            var actual = await _sut.LoadLink<SubContentsOwnerLinkedSource>().FromModelAsync(
                new SubContentsOwner
                {
                    Id = "1",
                    SubContents = null, //dont-care
                    SubSubContents = null
                }
            );

            Assert.Empty(actual.SubSubContents);
        }


        [Fact]
        public async Task LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicates()
        {
            var actual = await _sut.LoadLink<SubContentsOwnerLinkedSource>().FromModelAsync(
                new SubContentsOwner
                {
                    Id = "1",
                    SubContents = null, //dont-care
                    SubSubContents = new List<SubSubContent>
                    {
                        new SubSubContent { SummaryImageId = "a" },
                        new SubSubContent { SummaryImageId = "a" }
                    }
                }
            );

            var linkedImagesIds = actual.SubSubContents.Select(subSubContent => subSubContent.Model.SummaryImageId);
            Assert.Equal(new[] {"a", "a"}, linkedImagesIds);
        }

    }

    public class SubContentsOwnerLinkedSource : ILinkedSource<SubContentsOwner>
    {
        public SubContentsOwner Model { get; set; }
        public List<SubContentWithManySubSubContentsLinkedSource> SubContents { get; set; }
        public List<SubSubContentLinkedSource> SubSubContents { get; set; }
    }

    public class SubContentWithManySubSubContentsLinkedSource : ILinkedSource<SubContentWithManySubSubContents>
    {
        public SubContentWithManySubSubContents Model { get; set; }
        public List<SubSubContentLinkedSource> SubSubContents { get; set; }
    }

    public class SubContentsOwner
    {
        public string Id { get; set; }
        public List<SubContentWithManySubSubContents> SubContents { get; set; }
        public List<SubSubContent> SubSubContents { get; set; }
    }

    public class SubContentWithManySubSubContents
    {
        public List<SubSubContent> SubSubContents { get; set; }
    }
}