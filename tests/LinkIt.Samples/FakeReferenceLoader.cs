// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.PublicApi;
using LinkIt.Samples.Models;

namespace LinkIt.Samples
{
    public class FakeReferenceLoader : IReferenceLoader
    {
        public Task LoadReferencesAsync(ILoadingContext loadingContext)
        {
            foreach (var referenceType in loadingContext.GetReferenceTypes())
            {
                LoadReference(referenceType, loadingContext);
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //In case you need to dispose database connections or other ressources.

            //Will always be invoked as soon as the load phase is completed or
            //if an exception is thrown
        }

        private void LoadReference(Type referenceType, ILoadingContext loadingContext)
        {
            if (referenceType == typeof(Media)) LoadMedia(loadingContext);
            if (referenceType == typeof(Tag)) LoadTags(loadingContext);
            if (referenceType == typeof(BlogPost)) LoadBlogPosts(loadingContext);
            if (referenceType == typeof(Image)) LoadImages(loadingContext);
        }

        private void LoadMedia(ILoadingContext loadingContext)
        {
            var lookupIds = loadingContext.GetReferenceIds<Media, int>();
            var references = lookupIds.Select(id =>
                    new Media{
                        Id = id,
                        Title = "title-" + id,
                        TagIds = new List<int>
                        {
                            1000 + id,
                            1001 + id
                        }
                    }
                )
                .ToList();

            loadingContext.AddReferences(
                references,
                reference => reference.Id
            );
        }

        private void LoadTags(ILoadingContext loadingContext)
        {
            var lookupIds = loadingContext.GetReferenceIds<Tag, int>();
            var references = lookupIds
                .Select(id =>
                    new Tag
                    {
                        Id = id,
                        Name = id + "-name"
                    }
                )
                .ToList();

            loadingContext.AddReferences(
                references,
                reference => reference.Id
            );
        }

        private void LoadBlogPosts(ILoadingContext loadingContext)
        {
            var lookupIds = loadingContext.GetReferenceIds<BlogPost, int>();
            var references = lookupIds
                .Select(id =>
                    new BlogPost
                    {
                        Id = id,
                        Author = new Author
                        {
                            Name = "author-name-" + id,
                            Email = "author-email-" + id,
                            ImageId = "id-" + (id + 500)
                        },
                        MultimediaContentRef = new MultimediaContentReference
                        {
                            Type = id % 2 == 0
                                ? "image"
                                : "media",
                            Id = id % 2 == 0
                                ? "id-" + id
                                : (object) id
                        },
                        TagIds = new List<int>
                        {
                            88 + id,
                            89 + id
                        },
                        Title = "Title-" + id
                    }
                )
                .ToList();

            loadingContext.AddReferences(
                references,
                reference => reference.Id
            );
        }

        private void LoadImages(ILoadingContext loadingContext)
        {
            var lookupIds = loadingContext.GetReferenceIds<Image, string>();
            var references = lookupIds
                .Select(id =>
                    new Image
                    {
                        Id = id,
                        Credits = id + "-credits",
                        Url = id + "-url"
                    }
                )
                .ToList();

            loadingContext.AddReferences(
                references,
                reference => reference.Id
            );
        }
    }
}