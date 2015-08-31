using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class SubLinkedSourceTests
    {
        private LoadLinkProtocolFactory<SubContentOwner, string> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp()
        {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<SubContentOwner, string>(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<SubContentOwnerLinkedSource, SubContentOwner, string>(),
                    new SubLinkedSourceLoadLinkExpression<SubContentOwnerLinkedSource, SubContentLinkedSource, SubContent>(
                        linkedSource => linkedSource.Model.SubContent,
                        (linkedSource, subLinkedSource) => linkedSource.SubContent = subLinkedSource),
                    new SubLinkedSourceLoadLinkExpression<SubContentOwnerLinkedSource, SubSubContentLinkedSource, SubSubContent>(
                        linkedSource => linkedSource.Model.SubSubContent,
                        (linkedSource, subLinkedSource) => linkedSource.SubSubContent = subLinkedSource),
                    new SubLinkedSourceLoadLinkExpression<SubContentLinkedSource, SubSubContentLinkedSource, SubSubContent>(
                        linkedSource => linkedSource.Model.SubSubContent,
                        (linkedSource, subLinkedSource) => linkedSource.SubSubContent = subLinkedSource),
                    new ReferenceLoadLinkExpression<SubSubContentLinkedSource, Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference)
                },
                getReferenceIdFunc: reference => reference.Id,
                fakeReferenceTypeForLoadingLevel: new[] {
                    new List<Type>{typeof(SubContentOwner)},
                    new List<Type>{typeof(Image)},
                }
                );
        }

        [Test]
        public void LoadLink_SubLinkedSource()
        {
            var sut = _loadLinkProtocolFactory.Create(
                new SubContentOwner {
                    Id = "1",
                    SubContent = new SubContent{
                        SubSubContent = new SubSubContent{
                            SummaryImageId = "a"
                        }
                    },
                    SubSubContent = new SubSubContent{
                        SummaryImageId = "b"
                    }
                }
            );

            var actual = sut.LoadLink<SubContentOwnerLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        //[Test]
        //public void LoadLink_SingleReferenceWithoutReferenceId_ShouldLinkNull() {
        //    var sut = _loadLinkProtocolFactory.Create(
        //        new SingleReferenceContent {
        //            Id = "1",
        //            SummaryImageId = null
        //        }
        //    );

        //    var actual = sut.LoadLink<SingleReferenceLinkedSource>("1");

        //    Assert.That(actual.SummaryImage, Is.Null);
        //}

        //[Test]
        //public void LoadLink_SingleReferenceCannotBeResolved_ShouldLinkNull() {
        //    var sut = _loadLinkProtocolFactory.Create(
        //        new SingleReferenceContent {
        //            Id = "1",
        //            SummaryImageId = "cannot-be-resolved"
        //        }
        //    );

        //    var actual = sut.LoadLink<SingleReferenceLinkedSource>("1");

        //    Assert.That(actual.SummaryImage, Is.Null);
        //}
    }

    public class SubContentOwnerLinkedSource : ILinkedSource<SubContentOwner> {
        public SubContentOwner Model { get; set; }
        public SubContentLinkedSource SubContent { get; set; }
        public SubSubContentLinkedSource SubSubContent { get; set; }
    }

    public class SubContentLinkedSource : ILinkedSource<SubContent> {
        public SubContent Model { get; set; }
        public SubSubContentLinkedSource SubSubContent { get; set; }
    }

    public class SubSubContentLinkedSource : ILinkedSource<SubSubContent> {
        public SubSubContent Model { get; set; }
        public Image SummaryImage { get; set; }
    }

    public class SubContentOwner {
        public string Id { get; set; }
        public SubContent SubContent { get; set; }
        public SubSubContent SubSubContent { get; set; }
    }

    public class SubContent {
        public SubSubContent SubSubContent { get; set; }
    }

    public class SubSubContent {
        public string SummaryImageId { get; set; }
    }

}