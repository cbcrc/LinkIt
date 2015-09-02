using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class SubLinkedSourcesTests
    {
        private LoadLinkProtocolFactory<SubContentsOwner, string> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp()
        {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<SubContentsOwner, string>(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<SubContentsOwnerLinkedSource, SubContentsOwner, string>(),
                    new SubLinkedSourcesLoadLinkExpression<SubContentsOwnerLinkedSource, SubContentWithManySubSubContentsLinkedSource, SubContentWithManySubSubContents>(
                        linkedSource => linkedSource.Model.SubContents,
                        (linkedSource, subLinkedSources) => linkedSource.SubContents = subLinkedSources),
                    new SubLinkedSourcesLoadLinkExpression<SubContentsOwnerLinkedSource, SubSubContentLinkedSource, SubSubContent>(
                        linkedSource => linkedSource.Model.SubSubContents,
                        (linkedSource, subLinkedSources) => linkedSource.SubSubContents = subLinkedSources),
                    new SubLinkedSourcesLoadLinkExpression<SubContentWithManySubSubContentsLinkedSource, SubSubContentLinkedSource, SubSubContent>(
                        linkedSource => linkedSource.Model.SubSubContents,
                        (linkedSource, subLinkedSources) => linkedSource.SubSubContents = subLinkedSources),
                    new ReferenceLoadLinkExpression<SubSubContentLinkedSource, Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference)
                },
                getReferenceIdFunc: reference => reference.Id
                );
        }

        [Test]
        public void LoadLink_SubLinkedSources()
        {
            var sut = _loadLinkProtocolFactory.Create(
                new SubContentsOwner {
                    Id = "1",
                    SubContents = new List<SubContentWithManySubSubContents>{
                        new SubContentWithManySubSubContents{
                            SubSubContents = new List<SubSubContent>{
                                new SubSubContent{ SummaryImageId = "a" },
                                new SubSubContent{ SummaryImageId = "b" }
                            }
                        }
                    },
                    SubSubContents = new List<SubSubContent>{
                        new SubSubContent{ SummaryImageId = "c" },
                        new SubSubContent{ SummaryImageId = "d" }
                    }
                }
            );

            var actual = sut.LoadLink<SubContentsOwnerLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_SubLinkedSourcesWithNullInReferenceIds_ShouldIgnoreNull() {
            var sut = _loadLinkProtocolFactory.Create(
                new SubContentsOwner {
                    Id = "1",
                    SubContents = null, //dont-care
                    SubSubContents = new List<SubSubContent>{
                        new SubSubContent{ SummaryImageId = "c" },
                        null,
                        new SubSubContent{ SummaryImageId = "d" }
                    }
                }
            );

            var actual = sut.LoadLink<SubContentsOwnerLinkedSource>("1");

            Assert.That(actual.SubSubContents.Count, Is.EqualTo(2));
        }

        [Test]
        public void LoadLink_SubLinkedSourcesWithoutReferenceIds_ShouldLinkEmptySet() {
            var sut = _loadLinkProtocolFactory.Create(
                new SubContentsOwner {
                    Id = "1",
                    SubContents = null, //dont-care
                    SubSubContents =null
                }
            );

            var actual = sut.LoadLink<SubContentsOwnerLinkedSource>("1");

            Assert.That(actual.SubSubContents, Is.Empty);
        }


        [Test]
        public void LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            var sut = _loadLinkProtocolFactory.Create(
                new SubContentsOwner {
                    Id = "1",
                    SubContents = null, //dont-care
                    SubSubContents = new List<SubSubContent>{
                        new SubSubContent{ SummaryImageId = "a" },
                        new SubSubContent{ SummaryImageId = "a" }
                    }
                }
            );

            var actual = sut.LoadLink<SubContentsOwnerLinkedSource>("1");

            var linkedImagesIds = actual.SubSubContents.Select(subSubContent => subSubContent.Model.SummaryImageId);
            Assert.That(linkedImagesIds, Is.EquivalentTo(new[] { "a", "a" }));
        }
    }

    public class SubContentsOwnerLinkedSource : ILinkedSource<SubContentsOwner> {
        public SubContentsOwner Model { get; set; }
        public List<SubContentWithManySubSubContentsLinkedSource> SubContents { get; set; }
        public List<SubSubContentLinkedSource> SubSubContents { get; set; }
    }

    public class SubContentWithManySubSubContentsLinkedSource : ILinkedSource<SubContentWithManySubSubContents> {
        public SubContentWithManySubSubContents Model { get; set; }
        public List<SubSubContentLinkedSource> SubSubContents { get; set; }
    }

    public class SubContentsOwner {
        public string Id { get; set; }
        public List<SubContentWithManySubSubContents> SubContents { get; set; }
        public List<SubSubContent> SubSubContents { get; set; }
    }

    public class SubContentWithManySubSubContents {
        public List<SubSubContent> SubSubContents { get; set; }
    }

}