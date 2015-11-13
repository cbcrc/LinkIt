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
    public class ManyReferencesTests
    {
        private FakeReferenceLoader<ManyReferencesContent, int> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

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

            _fakeReferenceLoader =
                new FakeReferenceLoader<ManyReferencesContent, int>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_ManyReferences()
        {
            _fakeReferenceLoader.FixValue(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "summary-image-id",
                    AuthorImageId = "author-image-id",
                    FavoriteImageIds = new List<string> { "one", "two" }
                }
            );

            var actual = _sut.LoadLink<ManyReferencesLinkedSource>().ById(1);
            
            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_ManyReferencesWithNullInReferenceIds_ShouldLinkNull() {
            _fakeReferenceLoader.FixValue(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = new List<string> { "one", null, "two" }
                }
            );

            var actual = _sut.LoadLink<ManyReferencesLinkedSource>().ById(1);

            Assert.That(actual.FavoriteImages.Count, Is.EqualTo(3));
            Assert.That(actual.FavoriteImages[1], Is.Null);
        }

        [Test]
        public void LoadLink_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySet() {
            _fakeReferenceLoader.FixValue(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = null
                }
            );

            var actual = _sut.LoadLink<ManyReferencesLinkedSource>().ById(1);

            Assert.That(actual.FavoriteImages, Is.Empty);
        }

        [Test]
        public void LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            _fakeReferenceLoader.FixValue(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = new List<string> { "a", "a" }
                }
            );

            var actual = _sut.LoadLink<ManyReferencesLinkedSource>().ById(1);

            var linkedImagesIds = actual.FavoriteImages.Select(image => image.Id);
            Assert.That(linkedImagesIds, Is.EquivalentTo(new []{"a", "a"}));
        }


        [Test]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            _fakeReferenceLoader.FixValue(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "dont-care",
                    AuthorImageId = "dont-care",
                    FavoriteImageIds = new List<string> { "cannot-be-resolved", "cannot-be-resolved" }
                }
            );

            var actual = _sut.LoadLink<ManyReferencesLinkedSource>().ById(1);

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