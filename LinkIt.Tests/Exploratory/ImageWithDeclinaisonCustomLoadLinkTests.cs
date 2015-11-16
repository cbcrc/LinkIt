using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.Protocols;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Exploratory {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ImageWithDeclinaisonCustomLoadLinkTests
    {
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithImageLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ImageUrl,
                    linkedSource => linkedSource.Image
                );

            _sut = loadLinkProtocolBuilder.Build(() =>
                new ReferenceLoaderStub(new ImageReferenceTypeConfigWorkAround())
            );
        }

        [Test]
        public void LoadLink_ImagesFromDeclinaisonUrl()
        {
            var actual = _sut.LoadLink<WithImageLinkedSource>().FromModel(
                new WithImage {
                    Id = "1",
                    ImageUrl = "a-1x1"
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_ImagesFromDeclinaisonUrlWithoutReferenceId_ShouldLinkNull() {
            var actual = _sut.LoadLink<WithImageLinkedSource>().FromModel(
                new WithImage {
                    Id = "1",
                    ImageUrl = null
                }
            );

            Assert.That(actual.Image, Is.Null);
        }

        [Test]
        public void LoadLink_ImagesFromDeclinaisonUrlCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<WithImageLinkedSource>().FromModel(
                new WithImage {
                    Id = "1",
                    ImageUrl = "cannot-be-resolved"
                }
            );

            Assert.That(actual.Image, Is.Null);
        }

    }

    public class WithImageLinkedSource : ILinkedSource<WithImage>
    {
        public WithImage Model { get; set; }
        public ImageWithDeclinaison Image { get; set; }
    }

    public class WithImage{
        public string Id { get; set; }
        public string ImageUrl { get; set; }
    }

    public class ImageWithDeclinaison {
        public string Alt { get; set; }
        public List<Declinaison> Declinaisons { get; set; }
    }

    public class Declinaison {
        public string Url { get; set; }
        public string Ratio { get; set; }
    }

    public class ImageWithDeclinaisonRepository
    {
        private static readonly Dictionary<string, ImageWithDeclinaison> _imagesByDeclinaisonUrl;

        static ImageWithDeclinaisonRepository()
        {
            var images = new[]
            {
                new ImageWithDeclinaison
                {
                    Alt = "alt-a",
                    Declinaisons = new List<Declinaison>()
                    {
                        new Declinaison {Url = "a-16x9", Ratio = "16x9"},
                        new Declinaison {Url = "a-1x1", Ratio = "1x1"}
                    }
                }
            };

            _imagesByDeclinaisonUrl = images
                .SelectMany(image=>image.Declinaisons
                    .Select(declinaison => new {declinaison.Url, Image=image})
                )
                .ToDictionary(
                    item=>item.Url,
                    item=>item.Image
                );
        }

        public List<ImageWithDeclinaison> GetByDeclinaisonUrl(List<string> declinaisonUrls)
        {
            return declinaisonUrls
                .Where(declinaisonUrl => _imagesByDeclinaisonUrl.ContainsKey(declinaisonUrl))
                .Select(declinaisonUrl => _imagesByDeclinaisonUrl[declinaisonUrl])
                .Distinct()
                .ToList();
        }
    }

    public class ImageReferenceTypeConfigWorkAround: IReferenceTypeConfig
    {
        public Type ReferenceType
        {
            get { return typeof (ImageWithDeclinaison); }
        }

        public void Load(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext)
        {
            var lookupIds = lookupIdContext.GetReferenceIds<ImageWithDeclinaison, string>();
            var repository = new ImageWithDeclinaisonRepository();
            var images = repository.GetByDeclinaisonUrl(lookupIds);

            var imagesByDeclinaisonUrl = images
                .SelectMany(
                    image => image.Declinaisons
                        .Select(declinaison => new {
                            DeclinaisonUrl = declinaison.Url,
                            Image = image
                        })
                )
                .ToDictionary(
                    imageByDeclinaisonUrl => imageByDeclinaisonUrl.DeclinaisonUrl,
                    imageByDeclinaisonUrl => imageByDeclinaisonUrl.Image
                );

            loadedReferenceContext.AddReferences(imagesByDeclinaisonUrl);    
        }

        public string RequiredConnection
        {
            get { return null; }
        }
    }
}
