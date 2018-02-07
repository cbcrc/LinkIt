// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Exploratory
{
    public class NestedPolymorphicReferenceTests
    {
        public NestedPolymorphicReferenceTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithNestedPolymorphicReferenceLinkedSource>()
                .LoadLinkPolymorphicList(
                    linkedSource => linkedSource.Model.PolyIds,
                    linkedSource => linkedSource.Contents,
                    reference => reference.GetType(),
                    includes => includes.Include<PersonLinkedSource>().AsNestedLinkedSourceById(
                            typeof(string),
                            reference => (string) reference)
                        .Include<ImageWithContextualizationLinkedSource>().AsNestedLinkedSourceById(
                            typeof(int),
                            reference => ((int) reference).ToString()));

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        private readonly ILoadLinkProtocol _sut;

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_NestedPolymorphicReferenceAsync()
        {
            var actual = await _sut.LoadLink<WithNestedPolymorphicReferenceLinkedSource>().FromModelAsync(
                new WithNestedPolymorphicReference
                {
                    Id = "1",
                    PolyIds = new List<object> { "p1", 32 }
                }
            );

            Assert.Collection(
                actual.Contents,
                linkedSource =>
                {
                    var personLinkedSource = Assert.IsType<PersonLinkedSource>(linkedSource);
                    Assert.Equal("p1", personLinkedSource.Model.Id);
                },
                linkedSource =>
                {
                    var imageLinkedSource = Assert.IsType<ImageWithContextualizationLinkedSource>(linkedSource);
                    Assert.Equal("32", imageLinkedSource.Model.Id);
                }
            );
        }

        public class WithNestedPolymorphicReferenceLinkedSource : ILinkedSource<WithNestedPolymorphicReference>
        {
            public List<object> Contents { get; set; }
            public WithNestedPolymorphicReference Model { get; set; }
        }

        public class WithNestedPolymorphicReference
        {
            public string Id { get; set; }
            public List<object> PolyIds { get; set; }
        }
    }
}