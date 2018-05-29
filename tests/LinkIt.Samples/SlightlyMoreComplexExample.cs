// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.Samples.LinkedSources;
using LinkIt.Samples.Models;
using Xunit;

namespace LinkIt.Samples
{
    public class SlightlyMoreComplexExample: IClassFixture<LoadLinkProtocolFixture>
    {
        private readonly LoadLinkProtocolFixture _fixture;

        public SlightlyMoreComplexExample(LoadLinkProtocolFixture fixture)
        {
            _fixture = fixture;
        }

        private List<BlogPost> GetBlogPosts()
        {
            //fake result of a database query
            return new List<BlogPost>
            {
                new BlogPost
                {
                    Id = 77,
                    Author = new Author
                    {
                        Name = "author-name-77",
                        Email = "author-email-77",
                        ImageId = "id-77"
                    },
                    MultimediaContentRef = new MultimediaContentReference
                    {
                        Type = "media",
                        Id = 277
                    },
                    TagIds = new List<int>
                    {
                        177,
                        178
                    },
                    Title = "Salmon"
                },
                new BlogPost
                {
                    Id = 78,
                    Author = new Author
                    {
                        Name = "author-name-78",
                        Email = "author-email-78",
                        ImageId = "id-78"
                    },
                    MultimediaContentRef = new MultimediaContentReference
                    {
                        Type = "image",
                        Id = "id-123"
                    },
                    TagIds = new List<int>
                    {
                        178
                    },
                    Title = "Cod"
                }
            };
        }

        [Fact]
        public async Task LoadLink_FromQuery()
        {
            var models = GetBlogPosts();
            var actual = await _fixture.LoadLinkProtocol.LoadLink<BlogPostLinkedSource>().FromModelsAsync(models);

            Assert.Equal(models.Count, actual.Count);

            AssertLinkedSourceForModel(models[0], actual[0]);
            AssertLinkedSourceForModel(models[1], actual[1]);
        }

        [Fact]
        public async Task LoadLink_FromTransiantModel()
        {
            var model = new BlogPost
            {
                Id = 101,
                Author = new Author
                {
                    Name = "author-name-101",
                    Email = "author-email-101",
                    ImageId = "distinc-id-loaded-once" //same entity referenced twice
                },
                MultimediaContentRef = new MultimediaContentReference
                {
                    Type = "image",
                    Id = "distinc-id-loaded-once" //same entity referenced twice
                },
                TagIds = new List<int>
                {
                    1001,
                    1002
                },
                Title = "Title-101"
            };
            var actual = await _fixture.LoadLinkProtocol.LoadLink<BlogPostLinkedSource>().FromModelAsync(model);

            AssertLinkedSourceForModel(model, actual);
        }

        [Fact]
        public async Task LoadLinkById()
        {
            var actual = await _fixture.LoadLinkProtocol.LoadLink<BlogPostLinkedSource>().ByIdAsync(1);

            Assert.Equal(1, actual.Model.Id);
            AssertLinkedSourceForModel(actual.Model, actual);
        }

        [Fact]
        public async Task LoadLinkByIds()
        {
            var actual = (await _fixture.LoadLinkProtocol.LoadLink<BlogPostLinkedSource>().ByIdsAsync(
                new List<int> { 3, 2, 1 }
            )).OrderBy(x => x.Model.Id).ToList();

            Assert.Collection(
                actual,
                linkedSource => { Assert.Equal(1, linkedSource.Model.Id); AssertLinkedSourceForModel(linkedSource.Model, linkedSource); },
                linkedSource => { Assert.Equal(2, linkedSource.Model.Id); AssertLinkedSourceForModel(linkedSource.Model, linkedSource); },
                linkedSource => { Assert.Equal(3, linkedSource.Model.Id); AssertLinkedSourceForModel(linkedSource.Model, linkedSource); }
            );
        }

        private static void AssertLinkedSourceForModel(BlogPost model, BlogPostLinkedSource linkedSource)
        {
            Assert.Same(model, linkedSource.Model);
            Assert.Collection(
                linkedSource.Tags,
                model.TagIds.Select<int, Action<Tag>>(tagId => (t => Assert.Equal(tagId, t.Id))).ToArray()
            );
            Assert.Same(model.Author, linkedSource.Author.Model);
            Assert.Equal(model.Author.ImageId, linkedSource.Author.Image.Id);

            if (model.MultimediaContentRef.Type == "image")
            {
                Assert.IsType<Image>(linkedSource.MultimediaContent);
                var image = (Image) linkedSource.MultimediaContent;
                Assert.Equal(model.MultimediaContentRef.Id, image.Id);
            }
            else if (model.MultimediaContentRef.Type == "media")
            {
                Assert.IsType<MediaLinkedSource>(linkedSource.MultimediaContent);
                var mediaLinkedSource = (MediaLinkedSource) linkedSource.MultimediaContent;
                Assert.Equal(model.MultimediaContentRef.Id, mediaLinkedSource.Model.Id);
            }
        }
    }
}