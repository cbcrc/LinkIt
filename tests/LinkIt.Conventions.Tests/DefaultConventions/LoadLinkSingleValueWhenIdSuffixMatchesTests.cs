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
    public class LoadLinkSingleValueWhenIdSuffixMatchesTests
    {
        [Fact]
        public async Task GetLinkedSourceTypes()
        {
            var model = new Model
            {
                Id = "One",
                MediaReferenceId = 1,
                MediaNestedLinkedSourceId = 2
            };
            var sut = BuildLoadLinkProtocol();

            var actual = await sut.LoadLink<LinkedSource>().FromModelAsync(model);

            Assert.Same(model, actual.Model);
            Assert.Equal(model.MediaNestedLinkedSourceId, actual.MediaNestedLinkedSource.Model.Id);
            Assert.Equal(model.MediaReferenceId, actual.MediaReference.Id);
        }

        private static ILoadLinkProtocol BuildLoadLinkProtocol()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkSingleValueWhenIdSuffixMatches() }
            );

            return loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public Media MediaReference { get; set; }
            public MediaLinkedSource MediaNestedLinkedSource { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public int MediaReferenceId { get; set; }
            public int MediaNestedLinkedSourceId { get; set; }
        }
    }
}