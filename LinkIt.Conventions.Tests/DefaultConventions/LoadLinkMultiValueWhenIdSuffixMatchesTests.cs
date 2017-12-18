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


namespace LinkIt.Conventions.Tests.DefaultConventions
{
    public class LoadLinkMultiValueWhenIdSuffixMatchesTests {
        [Fact]
        public void GetLinkedSourceTypes(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            
            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkMultiValueWhenIdSuffixMatches() }
            );

            var sut = loadLinkProtocolBuilder.Build(()=>new ReferenceLoaderStub());

            var actual = sut.LoadLink<LinkedSource>().FromModel(
                new Model{
                    Id="One",
                    MediaReferenceIds = new List<int> {1, 11},
                    MediaNestedLinkedSourceIds = new List<int> {2, 22}
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public List<Media> MediaReferences { get; set; }
            public List<MediaLinkedSource> MediaNestedLinkedSources { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public List<int> MediaReferenceIds { get; set; }
            public List<int> MediaNestedLinkedSourceIds { get; set; }
        }
    }
}
