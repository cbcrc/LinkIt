using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class SubLinkedSourceTests
    {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<SubContentOwnerLinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubContent,
                    linkedSource => linkedSource.SubContent
                )
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubSubContent,
                    linkedSource => linkedSource.SubSubContent
                );
            loadLinkProtocolBuilder.For<SubContentLinkedSource>()
                .LoadLinkNestedLinkedSourceFromModel(
                    linkedSource => linkedSource.Model.SubSubContent,
                    linkedSource => linkedSource.SubSubContent
                );
            loadLinkProtocolBuilder.For<SubSubContentLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_SubLinkedSource()
        {
            var actual = _sut.LoadLink<SubContentOwnerLinkedSource>().FromModel(
                new SubContentOwner {
                    Id = "1",
                    SubContent = new SubContent {
                        SubSubContent = new SubSubContent {
                            SummaryImageId = "a"
                        }
                    },
                    SubSubContent = new SubSubContent {
                        SummaryImageId = "b"
                    }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_SingleReferenceWithoutReferenceId_ShouldLinkNull() {
            var actual = _sut.LoadLink<SubContentOwnerLinkedSource>().FromModel(
                new SubContentOwner {
                    Id = "1",
                    SubContent = new SubContent {
                        SubSubContent = null
                    },
                    SubSubContent = null
                }
            );

            Assert.That(actual.SubContent.SubSubContent, Is.Null);
            Assert.That(actual.SubSubContent, Is.Null);
        }
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