using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Exploratory {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ImageWithDeclinaisonCustomLoadLinkTests
    {
        private FakeReferenceLoader<WithImage, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithImageLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.ImageUrl,
                    linkedSource => linkedSource.Image
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<WithImage, string>(
                    reference=>reference.Id,
                    new ImageReferenceTypeConfigWorkAround()
                );
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_ImagesFromDeclinaisonUrl()
        {
            _fakeReferenceLoader.FixValue(
                new WithImage{
                    Id = "1",
                    ImageUrl = "a-1x1"
                }
            );

            var actual = _sut.LoadLink<WithImageLinkedSource>().ById("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }

    public class WithImageLinkedSource : ILinkedSource<WithImage>
    {
        public WithImage Model { get; set; }
        public ImageWithDeclinaison Image { get; set; }
        public Declinaison SelectedDeclinaison { get; set; }
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
            var references = new ImageWithDeclinaisonRepository();
            var images = references.GetByDeclinaisonUrl(lookupIds);

            foreach (var image in images){
                foreach (var declinaison in image.Declinaisons){
                    loadedReferenceContext.AddReference(image, declinaison.Url);    
                }
            }
        }

        public string RequiredConnection
        {
            get { return null; }
        }
    }
}
