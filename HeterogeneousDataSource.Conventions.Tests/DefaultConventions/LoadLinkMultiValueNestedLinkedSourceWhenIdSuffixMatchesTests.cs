using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSource.Conventions.DefaultConventions;
using HeterogeneousDataSource.Conventions.Interfaces;
using HeterogeneousDataSources;
using HeterogeneousDataSources.Tests;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSource.Conventions.Tests.DefaultConventions
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkMultiValueNestedLinkedSourceWhenIdSuffixMatchesTests {
        [Test]
        public void GetLinkedSourceTypes(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            
            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkMultiValueNestedLinkedSourceWhenIdSuffixMatches() }
            );

            var fakeReferenceLoader =
                new FakeReferenceLoader<Model, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            var actual = sut.LoadLink<LinkedSource>().FromModel(
                new Model{
                    Id="One",
                    MediaIds = new List<int>{1,2}
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public List<MediaLinkedSource> Medias { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public List<int> MediaIds { get; set; }
        }
    }
}
