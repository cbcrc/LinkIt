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
    public class LoadLinkSingleValueSubLinkedSourceWhenNameMatchesTests
    {
        [Fact]
        public async Task GetLinkedSourceTypes()
        {
            var model = new Model
            {
                Id = "One",
                Media = new Media
                {
                    Id = 1
                }
            };
            var sut = BuildLoadLinkProtocol();

            var actual = await sut.LoadLink<LinkedSource>().FromModelAsync(model);

            Assert.Same(model, actual.Model);
            Assert.Equal(model.Media.Id, actual.Media.Model.Id);
        }

        private static ILoadLinkProtocol BuildLoadLinkProtocol()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkSingleValueNestedLinkedSourceFromModelWhenNameMatches() }
            );

            return loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public MediaLinkedSource Media { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public Media Media { get; set; }
        }
    }
}