using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class SingleReferenceTests
    {
        [Test]
        public void LoadLink_SingleReference()
        {
            var sut = TestHelper.CreateLoadLinkProtocol(
                loadLinkExpressions: GetLoadLinkExpressions(),
                fixedValue: new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "a"
                },
                getReferenceIdFunc: GetReferenceIdFunc(),
                fakeReferenceTypeForLoadingLevel: FakeReferenceTypeForLoadingLevel());

            var actual = sut.LoadLink<SingleReferenceLinkedSource, string, SingleReferenceContent>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_SingleReferenceWithoutReferenceId_ShouldLinkNull() {
            var sut = TestHelper.CreateLoadLinkProtocol(
                loadLinkExpressions: GetLoadLinkExpressions(),
                fixedValue: new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = null
                },
                getReferenceIdFunc: GetReferenceIdFunc(), 
                fakeReferenceTypeForLoadingLevel: FakeReferenceTypeForLoadingLevel());

            var actual = sut.LoadLink<SingleReferenceLinkedSource, string, SingleReferenceContent>("1");

            Assert.That(actual.SummaryImage, Is.Null);
        }

        [Test]
        public void LoadLink_SingleReferenceCannotBeResolved_ShouldLinkNull() {
            var sut = TestHelper.CreateLoadLinkProtocol(
                loadLinkExpressions: GetLoadLinkExpressions(),
                fixedValue: new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "cannot-be-resolved"
                },
                getReferenceIdFunc: GetReferenceIdFunc(),
                fakeReferenceTypeForLoadingLevel: FakeReferenceTypeForLoadingLevel());

            var actual = sut.LoadLink<SingleReferenceLinkedSource, string, SingleReferenceContent>("1");

            Assert.That(actual.SummaryImage, Is.Null);
        }

        private static List<Type>[] FakeReferenceTypeForLoadingLevel() {
            return new[] {
                new List<Type>{typeof(SingleReferenceContent)},
                new List<Type>{typeof(Image)},
            };
        }

        private static List<ILoadLinkExpression> GetLoadLinkExpressions()
        {
            return new List<ILoadLinkExpression>{
                new ReferenceLoadLinkExpression<SingleReferenceLinkedSource, Image, string>(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    (linkedSource, reference) => linkedSource.SummaryImage = reference
                    )
            };
        }

        private static Func<SingleReferenceContent, string> GetReferenceIdFunc()
        {
            return reference => reference.Id;
        }
    }

    public class SingleReferenceLinkedSource: ILinkedSource<SingleReferenceContent>
    {
        //stle: two steps loading sucks!
        public SingleReferenceContent Model { get; set; }
        public Image SummaryImage{ get; set; }
    }

    public class SingleReferenceContent {
        public string Id { get; set; }
        public string SummaryImageId { get; set; }
    }
}
