#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.DefaultConventions;
using LinkIt.Conventions.Interfaces;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;
using NUnit.Framework.Constraints;


namespace LinkIt.Conventions.Tests.DefaultConventions
{
    public class LoadLinkMultiValueSubLinkedSourceWhenNameMatchesTests {
        [Fact]
        public void GetLinkedSourceTypes(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            
            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkMultiValueNestedLinkedSourceFromModelWhenNameMatches() }
            );

            var sut = loadLinkProtocolBuilder.Build(()=>new ReferenceLoaderStub());

            var actual = sut.LoadLink<LinkedSource>().FromModel(
                new Model{
                    Id="One",
                    ListOfMedia = new List<Media>{
                        new Media {
                            Id = 1
                        },
                        new Media {
                            Id = 2
                        }
                    }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public List<MediaLinkedSource> ListOfMedia { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public List<Media> ListOfMedia { get; set; }
        }
    }
}
