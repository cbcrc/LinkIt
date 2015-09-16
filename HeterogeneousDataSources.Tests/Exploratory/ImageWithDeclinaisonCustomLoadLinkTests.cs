using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSources.Tests.Exploratory {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ImageWithDeclinaisonCustomLoadLinkTests
    {
        private FakeReferenceLoader2<WithContextualizedReference, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithContextualizedReferenceLinkedSource>()
                .IsRoot<string>();

            _fakeReferenceLoader =
                new FakeReferenceLoader2<WithContextualizedReference, string>(
                    reference => reference.Id,
                    new ReferenceTypeConfig<ImageWithDeclinaison, string>(
                        declinaisonUrls => new ImageWithDeclinaisonRepository().GetByDeclinaisonUrl(declinaisonUrls),
                        reference => reference.Alt //Stle: oups!: similar to query, not always ref-id==entity.id , sometimes one entity has many key and sometimes match should be specification(entity)==true
                    )
            );
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void X()
        {
           
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
}
