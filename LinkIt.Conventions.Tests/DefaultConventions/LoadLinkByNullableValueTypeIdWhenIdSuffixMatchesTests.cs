using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.DefaultConventions;
using LinkIt.Conventions.Interfaces;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Conventions.Tests.DefaultConventions
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkByNullableValueTypeIdWhenIdSuffixMatchesTests {
        [Test]
        public void GetLinkedSourceTypes(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            
            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkByNullableValueTypeIdWhenIdSuffixMatches() }
            );
            
            var sut = loadLinkProtocolBuilder.Build(()=>new ReferenceLoaderStub());

            var actual = sut.LoadLink<LinkedSource>().FromModel(
                new Model{
                    Id="One",
                    MediaReferenceId = 1,
                    MediaNestedLinkedSourceId = 2
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public Media MediaReference { get; set; }
            public Media MediaNestedLinkedSource { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public int? MediaReferenceId { get; set; }
            public int? MediaNestedLinkedSourceId { get; set; }
        }
    }
}
