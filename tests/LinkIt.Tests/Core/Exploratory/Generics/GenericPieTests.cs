// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FluentAssertions;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Exploratory.Generics
{
    public class GenericPieTests
    {
        public GenericPieTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<StringPieLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PieContent,
                    linkedSource => linkedSource.SummaryImage);

            loadLinkProtocolBuilder.For<IntPieLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.PieContent,
                    linkedSource => linkedSource.SummaryImage);

            _sut = loadLinkProtocolBuilder.Build(() =>
                new ReferenceLoaderStub(
                    new ReferenceTypeConfig<Pie<string>, string>(
                        ids => new PieRepository<string>().GetByPieContentIds(ids),
                        reference => reference.Id
                    ),
                    new ReferenceTypeConfig<Pie<int>, string>(
                        ids => new PieRepository<int>().GetByPieContentIds(ids),
                        reference => reference.Id
                    )
                )
            );
        }

        private readonly ILoadLinkProtocol _sut;

        [Fact]
        public async Task LoadLink_IntPie()
        {
            var actual = await _sut.LoadLink<IntPieLinkedSource>().ByIdAsync("2");

            Assert.Equal("2", actual.Model.Id);
            Assert.Equal("Int32", actual.SummaryImage.Id);
        }

        [Fact]
        public async Task LoadLink_StringPie()
        {
            var actual = await _sut.LoadLink<StringPieLinkedSource>().ByIdAsync("1");

            Assert.Equal("1", actual.Model.Id);
            Assert.Equal("String", actual.SummaryImage.Id);
        }
    }

    public class StringPieLinkedSource : ILinkedSource<Pie<string>>
    {
        public Image SummaryImage { get; set; }
        public Pie<string> Model { get; set; }
    }

    public class IntPieLinkedSource : ILinkedSource<Pie<int>>
    {
        public Image SummaryImage { get; set; }
        public Pie<int> Model { get; set; }
    }
}