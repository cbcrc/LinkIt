using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using LinkIt.ConfigBuilders;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.Protocols;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class SubLinkedSourcesTests
    {
        private FakeReferenceLoader<SubContentsOwner, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SubContentsOwnerLinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubContents,
                    linkedSource => linkedSource.SubContents
                )
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubSubContents,
                    linkedSource => linkedSource.SubSubContents
                );
            loadLinkProtocolBuilder.For<SubContentWithManySubSubContentsLinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubSubContents,
                    linkedSource => linkedSource.SubSubContents
                );
            loadLinkProtocolBuilder.For<SubSubContentLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<SubContentsOwner, string>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }



        [Test]
        public void LoadLink_SubLinkedSources()
        {
            _fakeReferenceLoader.FixValue(
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

            var actual = _sut.LoadLink<SubContentsOwnerLinkedSource>().ById("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_SubLinkedSourcesWithNullInReferenceIds_ShouldLinkNull() {
            _fakeReferenceLoader.FixValue(
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

            var actual = _sut.LoadLink<SubContentsOwnerLinkedSource>().ById("1");

            Assert.That(actual.SubSubContents.Count, Is.EqualTo(3));
            Assert.That(actual.SubSubContents[1], Is.Null);
        }

        [Test]
        public void LoadLink_SubLinkedSourcesWithoutReferenceIds_ShouldLinkEmptySet() {
            _fakeReferenceLoader.FixValue(
                new SubContentsOwner {
                    Id = "1",
                    SubContents = null, //dont-care
                    SubSubContents =null
                }
            );

            var actual = _sut.LoadLink<SubContentsOwnerLinkedSource>().ById("1");

            Assert.That(actual.SubSubContents, Is.Empty);
        }


        [Test]
        public void LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            _fakeReferenceLoader.FixValue(
                new SubContentsOwner {
                    Id = "1",
                    SubContents = null, //dont-care
                    SubSubContents = new List<SubSubContent>{
                        new SubSubContent{ SummaryImageId = "a" },
                        new SubSubContent{ SummaryImageId = "a" }
                    }
                }
            );

            var actual = _sut.LoadLink<SubContentsOwnerLinkedSource>().ById("1");

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