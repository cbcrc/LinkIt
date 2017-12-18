#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core
{
    public class MultipleReferencesTypeTests
    {
        private ILoadLinkProtocol _sut;

        public MultipleReferencesTypeTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<MultipleReferencesTypeLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage)
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.AuthorId,
                    linkedSource => linkedSource.Author);

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_MultipleReferencesTypeTestsAsync()
        {
            var actual = await _sut.LoadLink<MultipleReferencesTypeLinkedSource>().FromModelAsync(
                new MultipleReferencesTypeContent
                {
                    Id = 1,
                    SummaryImageId = "a",
                    AuthorId = "32"
                }
            );

            Assert.Equal("a", actual.SummaryImage.Id);
            Assert.Equal("32", actual.Author.Id);
        }
    }


    public class MultipleReferencesTypeLinkedSource : ILinkedSource<MultipleReferencesTypeContent>
    {
        public Image SummaryImage { get; set; }
        public Person Author { get; set; }
        public MultipleReferencesTypeContent Model { get; set; }
    }

    public class MultipleReferencesTypeContent
    {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
        public string AuthorId { get; set; }
    }
}