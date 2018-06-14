// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.DefaultConventions;
using LinkIt.Conventions.Interfaces;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Conventions.Tests.DefaultConventions
{
    public class LoadLinkNestedLinkedSourceListFromModelWhenNameMatchesTests
    {
        [Fact]
        public async Task GetLinkedSourceTypes()
        {
            var model = new Model
            {
                Id = "One",
                ListOfMedia = new List<Media>
                {
                    new Media
                    {
                        Id = 1
                    },
                    new Media
                    {
                        Id = 2
                    }
                }
            };
            var sut = BuildLoadLinkProtocol();

            var actual = await sut.LoadLink<LinkedSource>().FromModelAsync(model);

            Assert.Same(model, actual.Model);
            Assert.Collection(
                actual.ListOfMedia,
                mediaLinkedSource => Assert.Equal(model.ListOfMedia[0].Id, mediaLinkedSource.Model.Id),
                mediaLinkedSource => Assert.Equal(model.ListOfMedia[1].Id, mediaLinkedSource.Model.Id)
            );
        }

        private static ILoadLinkProtocol BuildLoadLinkProtocol()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkNestedLinkedSourceListFromModelWhenNameMatches() }
            );

            return loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public List<MediaLinkedSource> ListOfMedia { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public List<Media> ListOfMedia { get; set; }
        }
    }
}