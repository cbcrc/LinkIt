using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkConfigTests
    {
        [Test]
        public void X()
        {
            var sut = new LoadLinkConfig(
                new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<OneLoadingLevelContentLinkedSource, OneLoadingLevelContent, string>()
                },
                null //to remove
            );

            var numberOfLoadingLevel = sut.GetNumberOfLoadingLevel<OneLoadingLevelContentLinkedSource>();
            Assert.That(numberOfLoadingLevel, Is.EqualTo(1));

            var referenceTypeForLoadingLevel = sut.GetReferenceTypeForLoadingLevel<OneLoadingLevelContentLinkedSource>(0);
            Assert.That(
                referenceTypeForLoadingLevel, 
                Is.EquivalentTo(new[]{
                    typeof(OneLoadingLevelContent), 
                })
            );
        }

        public class OneLoadingLevelContentLinkedSource : ILinkedSource<OneLoadingLevelContent> {
            public OneLoadingLevelContent Model { get; set; }
        }

        public class OneLoadingLevelContent {
            public string Id { get; set; }
        }
    }
}
