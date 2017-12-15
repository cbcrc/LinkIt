#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.DefaultConventions;
using LinkIt.Conventions.Interfaces;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Conventions.Tests.DefaultConventions
{
    public class LoadLinkMultiValueWhenIdSuffixMatchesTests
    {
        [Fact]
        public void GetLinkedSourceTypes()
        {
            var model = new Model
            {
                Id = "One",
                MediaReferenceIds = new List<int> { 1, 11 },
                MediaNestedLinkedSourceIds = new List<int> { 2, 22 }
            };
            var sut = BuildLoadLinkProtocol();

            var actual = sut.LoadLink<LinkedSource>().FromModel(
                model
            );

            Assert.Same(model, actual.Model);
            Assert.Collection(
                actual.MediaReferences,
                media => Assert.Equal(model.MediaReferenceIds[0], media.Id),
                media => Assert.Equal(model.MediaReferenceIds[1], media.Id)
            );
            Assert.Collection(
                actual.MediaNestedLinkedSources,
                mediaLinkedSource => Assert.Equal(model.MediaNestedLinkedSourceIds[0], mediaLinkedSource.Model.Id),
                mediaLinkedSource => Assert.Equal(model.MediaNestedLinkedSourceIds[1], mediaLinkedSource.Model.Id)
            );
        }

        private static ILoadLinkProtocol BuildLoadLinkProtocol()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkMultiValueWhenIdSuffixMatches() }
            );

            return loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public List<Media> MediaReferences { get; set; }
            public List<MediaLinkedSource> MediaNestedLinkedSources { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public List<int> MediaReferenceIds { get; set; }
            public List<int> MediaNestedLinkedSourceIds { get; set; }
        }
    }
}