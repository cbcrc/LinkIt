using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.PublicApi;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ManyReferencesTests
    {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<ManyReferencesLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                )
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.AuthorImageId,
                    linkedSource => linkedSource.AuthorImage
                )
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.FavoriteImageIds,
                    linkedSource => linkedSource.FavoriteImages
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_ManyReferences()
        {
            var actual = _sut.LoadLink<ManyReferencesLinkedSource>().FromModel(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "summary-image-id",
                    AuthorImageId = "author-image-id",
                    FavoriteImageIds = new List<string> { "one", "two" }
                }
            );
            
            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_ManyReferencesWithNullInReferenceIds_ShouldLinkNull() {
            var actual = _sut.LoadLink<ManyReferencesLinkedSource>().FromModel(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = new List<string> { "one", null, "two" }
                }
            );

            Assert.That(actual.FavoriteImages.Count, Is.EqualTo(3));
            Assert.That(actual.FavoriteImages[1], Is.Null);
        }

        [Test]
        public void LoadLink_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySet() {
            var actual = _sut.LoadLink<ManyReferencesLinkedSource>().FromModel(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = null
                }
            );

            Assert.That(actual.FavoriteImages, Is.Empty);
        }

        [Test]
        public void LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            var actual = _sut.LoadLink<ManyReferencesLinkedSource>().FromModel(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = new List<string> { "a", "a" }
                }
            );

            var linkedImagesIds = actual.FavoriteImages.Select(image => image.Id);
            Assert.That(linkedImagesIds, Is.EquivalentTo(new []{"a", "a"}));
        }


        [Test]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<ManyReferencesLinkedSource>().FromModel(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = new List<string> { "cannot-be-resolved", "cannot-be-resolved" }
                }
            );

            Assert.That(actual.FavoriteImages, Is.EquivalentTo(new List<Image>{null,null}));
        }

    }

    public class ManyReferencesLinkedSource: ILinkedSource<ManyReferencesContent>{
        public ManyReferencesContent Model { get; set; }
        public Image SummaryImage { get; set; }
        public Image AuthorImage { get; set; }
        public List<Image> FavoriteImages { get; set; }
    }

    public class ManyReferencesContent {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
        public string AuthorImageId { get; set; }
        public List<string> FavoriteImageIds { get; set; }
    }

}