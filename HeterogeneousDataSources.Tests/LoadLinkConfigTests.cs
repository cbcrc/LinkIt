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
        public void GetReferenceTypeForLoadingLevel_OneLevel()
        {
            var sut = new LoadLinkConfig(
                new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<OneLoadingLevelContentLinkedSource, OneLoadingLevelContent, string>(),
                    new NestedLinkedSourceLoadLinkExpression<OneLoadingLevelContentRefererLinkedSource, OneLoadingLevelContentLinkedSource, OneLoadingLevelContent, string>(
                        linkedSource => linkedSource.Model.OneLoadingLevelContentId,
                        (linkedSource, childLinkedSource) => linkedSource.OneLoadingLevelContent = childLinkedSource
                    )
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

        public class OneLoadingLevelContentRefererLinkedSource: ILinkedSource<OneLoadingLevelContentReferer>
        {
            public OneLoadingLevelContentReferer Model { get; set; }
            public OneLoadingLevelContentLinkedSource OneLoadingLevelContent { get; set; }
        }

        public class OneLoadingLevelContentReferer {
            public string Id { get; set; }
            public string OneLoadingLevelContentId { get; set; }
        }

        [Test]
        public void GetReferenceTypeForLoadingLevel_ManyLevel() {
            var sut = new LoadLinkConfig(
                new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<ManyLoadingLevelContentLinkedSource, ManyLoadingLevelContent, string>(),
                    new NestedLinkedSourceLoadLinkExpression<ManyLoadingLevelContentLinkedSource, PersonLinkedSource, Person, string>(
                        linkedSource => linkedSource.Model.PersonId,
                        (linkedSource, nestedLinkedSource) => linkedSource.Person = nestedLinkedSource),
                    new ReferenceLoadLinkExpression<PersonLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference)
                },
                null //to remove
            );

            var numberOfLoadingLevel = sut.GetNumberOfLoadingLevel<ManyLoadingLevelContentLinkedSource>();
            Assert.That(numberOfLoadingLevel, Is.EqualTo(3));

            var referenceTypeForLoadingLevel0 = sut.GetReferenceTypeForLoadingLevel<ManyLoadingLevelContentLinkedSource>(0);
            Assert.That(
                referenceTypeForLoadingLevel0,
                Is.EquivalentTo(new[]{
                    typeof(ManyLoadingLevelContent), 
                })
            );
            var referenceTypeForLoadingLevel1 = sut.GetReferenceTypeForLoadingLevel<ManyLoadingLevelContentLinkedSource>(0);
            Assert.That(
                referenceTypeForLoadingLevel1,
                Is.EquivalentTo(new[]{
                    typeof(Person), 
                })
            );

            var referenceTypeForLoadingLevel2 = sut.GetReferenceTypeForLoadingLevel<ManyLoadingLevelContentLinkedSource>(0);
            Assert.That(
                referenceTypeForLoadingLevel2,
                Is.EquivalentTo(new[]{
                    typeof(Image), 
                })
            );

        }

        public class ManyLoadingLevelContentLinkedSource : ILinkedSource<ManyLoadingLevelContent> {
            public ManyLoadingLevelContent Model { get; set; }
            public PersonLinkedSource Person { get; set; }
        }

        public class ManyLoadingLevelContent {
            public string Id { get; set; }
            public string PersonId { get; set; }
        }
    }
}
