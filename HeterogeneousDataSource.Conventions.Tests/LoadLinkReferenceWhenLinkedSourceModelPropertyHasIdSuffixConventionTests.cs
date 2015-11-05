using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ApprovalTests.Reporters;
using HeterogeneousDataSources;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSource.Conventions.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkReferenceWhenLinkedSourceModelPropertyHasIdSuffixConventionTests {
        [Test]
        public void GetLinkedSourceTypes(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            
            loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new LoadLinkReferenceWhenLinkedSourceModelPropertyHasIdSuffixConvention() }
            );

            var fakeReferenceLoader =
                new FakeReferenceLoader<Model, string>(reference => reference.Id);
            var sut = loadLinkProtocolBuilder.Build(fakeReferenceLoader);

            var actual = sut.LoadLink<LinkedSource>().FromModel(
                new Model{
                    Id="One",
                    MandatoryMediaId = 1,
                    OptionMediaId = 2,
                    BestMediaIds = new List<int>{3,4}
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public Media MandatoryMedia { get; set; }
            public Media OptionMedia { get; set; }
            //public List<Media> BestMedias { get; set; }
        }

        public class Model{
            public string Id { get; set; }
            public int MandatoryMediaId { get; set; }
            public int? OptionMediaId { get; set; }
            public List<int> BestMediaIds { get; set; }
        }
    }
}
