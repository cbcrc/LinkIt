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
        private LoadLinkProtocolFactory<SingleReferenceContent, string> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp()
        {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<SingleReferenceContent, string>(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new ReferenceLoadLinkExpression<SingleReferenceLinkedSource, Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference
                    )
                },
                getReferenceIdFunc: reference => reference.Id,
                fakeReferenceTypeForLoadingLevel: new[] {
                    new List<Type>{typeof(SingleReferenceContent)},
                    new List<Type>{typeof(Image)},
                }
            );
        }

        [Test]
        public void LoadLink_SingleReference()
        {
            var sut = _loadLinkProtocolFactory.Create(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "a"
                }
            );

            var actual = sut.LoadLink<SingleReferenceLinkedSource, string, SingleReferenceContent>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_SingleReferenceWithoutReferenceId_ShouldLinkNull() {
            var sut = _loadLinkProtocolFactory.Create(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = null
                }
            );

            var actual = sut.LoadLink<SingleReferenceLinkedSource, string, SingleReferenceContent>("1");

            Assert.That(actual.SummaryImage, Is.Null);
        }

        [Test]
        public void LoadLink_SingleReferenceCannotBeResolved_ShouldLinkNull() {
            var sut = _loadLinkProtocolFactory.Create(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "cannot-be-resolved"
                }
            );

            var actual = sut.LoadLink<SingleReferenceLinkedSource, string, SingleReferenceContent>("1");

            Assert.That(actual.SummaryImage, Is.Null);
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
