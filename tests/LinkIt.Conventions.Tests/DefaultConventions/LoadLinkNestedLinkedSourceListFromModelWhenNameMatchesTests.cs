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

        [Fact]
        public async Task NestedLinkedSourceListInNestedLinkedSourceList()
        {
            var masterModel = new Master
            {
                ListOfModels = new List<Model>
                {
                     new Model
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
                    },
                    new Model
                    {
                        Id = "Two",
                        ListOfMedia = new List<Media>
                        {
                            new Media
                            {
                                Id = 3
                            },
                            new Media
                            {
                                Id = 4
                            }
                        }
                    }
                }
            };
            var sut = BuildLoadLinkProtocol();

            var actual = await sut.LoadLink<MasterLinkedSource>().FromModelAsync(masterModel);

            Assert.Same(masterModel, actual.Model);
            Assert.Collection(
                actual.ListOfModels,
                modelLinkedSource => Assert.Equal(masterModel.ListOfModels[0].Id, modelLinkedSource.Model.Id),
                modelLinkedSource => Assert.Equal(masterModel.ListOfModels[1].Id, modelLinkedSource.Model.Id)
            );
            Assert.Collection(
                actual.ListOfModels[0].ListOfMedia,
                mediaLinkedSource => Assert.Equal(masterModel.ListOfModels[0].ListOfMedia[0].Id, mediaLinkedSource.Model.Id),
                mediaLinkedSource => Assert.Equal(masterModel.ListOfModels[0].ListOfMedia[1].Id, mediaLinkedSource.Model.Id)
            );
            Assert.Collection(
                actual.ListOfModels[1].ListOfMedia,
                mediaLinkedSource => Assert.Equal(masterModel.ListOfModels[1].ListOfMedia[0].Id, mediaLinkedSource.Model.Id),
                mediaLinkedSource => Assert.Equal(masterModel.ListOfModels[1].ListOfMedia[1].Id, mediaLinkedSource.Model.Id)
            );
        }

        private static ILoadLinkProtocol BuildLoadLinkProtocol()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource), typeof(MasterLinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkNestedLinkedSourceListFromModelWhenNameMatches() }
            );

            return loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        public class MasterLinkedSource : ILinkedSource<Master>
        {
            public Master Model { get; set; }
            public List<LinkedSource> ListOfModels { get; set; }
        }

        public class Master
        {
            public List<Model> ListOfModels { get; set; }
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