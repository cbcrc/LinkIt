#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Tests.Core.Exploratory
{
    public class ImageWithRatiosCustomLoadLinkTests
    {
        public ImageWithRatiosCustomLoadLinkTests()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<WithImageLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.ImageUrl,
                    linkedSource => linkedSource.Image);

            _sut = loadLinkProtocolBuilder.Build(() =>
                new ReferenceLoaderStub(new ImageReferenceTypeConfigWorkAround())
            );
        }

        private readonly ILoadLinkProtocol _sut;

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ImagesFromDeclinaisonUrlAsync()
        {
            var actual = await _sut.LoadLink<WithImageLinkedSource>().FromModelAsync(
                new WithImage
                {
                    Id = "1",
                    ImageUrl = "a-1x1"
                }
            );

            Assert.Contains("a-1x1", actual.Image.Ratios.Select(r => r.Url));
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ImagesFromDeclinaisonUrlCannotBeResolved_ShouldLinkNullAsync()
        {
            var actual = await _sut.LoadLink<WithImageLinkedSource>().FromModelAsync(
                new WithImage
                {
                    Id = "1",
                    ImageUrl = "cannot-be-resolved"
                }
            );

            Assert.Null(actual.Image);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadLink_ImagesFromDeclinaisonUrlWithoutReferenceId_ShouldLinkNullAsync()
        {
            var actual = await _sut.LoadLink<WithImageLinkedSource>().FromModelAsync(
                new WithImage
                {
                    Id = "1",
                    ImageUrl = null
                }
            );

            Assert.Null(actual.Image);
        }
    }

    public class WithImageLinkedSource : ILinkedSource<WithImage>
    {
        public ImageWithRatios Image { get; set; }
        public WithImage Model { get; set; }
    }

    public class WithImage
    {
        public string Id { get; set; }
        public string ImageUrl { get; set; }
    }

    public class ImageWithRatios
    {
        public string Alt { get; set; }
        public List<ImageRatio> Ratios { get; set; }
    }

    public class ImageRatio
    {
        public string Url { get; set; }
        public string Ratio { get; set; }
    }

    public class ImageWithRatioRepository
    {
        private static readonly Dictionary<string, ImageWithRatios> _imagesByRatioUrl;

        static ImageWithRatioRepository()
        {
            var images = new[]
            {
                new ImageWithRatios
                {
                    Alt = "alt-a",
                    Ratios = new List<ImageRatio>
                    {
                        new ImageRatio { Url = "a-16x9", Ratio = "16x9" },
                        new ImageRatio { Url = "a-1x1", Ratio = "1x1" }
                    }
                }
            };

            _imagesByRatioUrl = images.SelectMany(image => image.Ratios.Select(declinaison => new { declinaison.Url, Image = image })
                )
                .ToDictionary(
                    item => item.Url,
                    item => item.Image);
        }

        public List<ImageWithRatios> GetByDeclinaisonUrl(IEnumerable<string> declinaisonUrls)
        {
            return declinaisonUrls.Where(declinaisonUrl => _imagesByRatioUrl.ContainsKey(declinaisonUrl))
                .Select(declinaisonUrl => _imagesByRatioUrl[declinaisonUrl])
                .Distinct()
                .ToList();
        }
    }

    public class ImageReferenceTypeConfigWorkAround : IReferenceTypeConfig
    {
        public Type ReferenceType => typeof(ImageWithRatios);

        public void Load(ILoadingContext loadingContext)
        {
            var lookupIds = loadingContext.GetReferenceIds<ImageWithRatios, string>();
            var repository = new ImageWithRatioRepository();
            var images = repository.GetByDeclinaisonUrl(lookupIds);

            var imagesByDeclinaisonUrl = images.SelectMany(
                    image => image.Ratios.Select(declinaison => new
                    {
                        DeclinaisonUrl = declinaison.Url,
                        Image = image
                    })
                )
                .ToDictionary(
                    imageByDeclinaisonUrl => imageByDeclinaisonUrl.DeclinaisonUrl,
                    imageByDeclinaisonUrl => imageByDeclinaisonUrl.Image);

            loadingContext.AddReferences(imagesByDeclinaisonUrl);
        }
    }
}