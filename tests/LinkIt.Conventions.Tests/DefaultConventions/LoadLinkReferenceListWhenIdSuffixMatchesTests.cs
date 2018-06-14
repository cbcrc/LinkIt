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
    public class LoadLinkReferenceListWhenIdSuffixMatchesTests
    {
        [Fact]
        public async Task GetLinkedSourceTypes()
        {
            var model = new Model
            {
                Id = "One",
                MediaReferenceIds = new List<int> { 1, 11 },
            };
            var sut = BuildLoadLinkProtocol();

            var actual = await sut.LoadLink<LinkedSource>().FromModelAsync(model);

            Assert.Same(model, actual.Model);
            Assert.Collection(
                actual.MediaReferences,
                media => Assert.Equal(model.MediaReferenceIds[0], media.Id),
                media => Assert.Equal(model.MediaReferenceIds[1], media.Id)
            );
        }

        private static ILoadLinkProtocol BuildLoadLinkProtocol()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention>
                {
                    new LoadLinkReferenceListWhenIdSuffixMatches(),
                }
            );

            return loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public List<Media> MediaReferences { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public List<int> MediaReferenceIds { get; set; }
        }
    }
}