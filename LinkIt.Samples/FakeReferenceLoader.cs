#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.PublicApi;
using LinkIt.Samples.Models;

namespace LinkIt.Samples
{
    public class FakeReferenceLoader : IReferenceLoader
    {
        public void LoadReferences(ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext)
        {
            foreach (var referenceType in lookupIdContext.GetReferenceTypes())
            {
                LoadReference(referenceType, lookupIdContext, loadedReferenceContext);
            }
        }

        public void Dispose()
        {
            //In case you need to dispose database connections or other ressources.

            //Will always be invoked as soon as the load phase is completed or
            //if an exception is thrown
        }

        private void LoadReference(Type referenceType, ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext)
        {
            if (referenceType == typeof(Media)) LoadMedia(lookupIdContext, loadedReferenceContext);
            if (referenceType == typeof(Tag)) LoadTags(lookupIdContext, loadedReferenceContext);
            if (referenceType == typeof(BlogPost)) LoadBlogPosts(lookupIdContext, loadedReferenceContext);
            if (referenceType == typeof(Image)) LoadImages(lookupIdContext, loadedReferenceContext);
        }

        private void LoadMedia(ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext)
        {
            var lookupIds = lookupIdContext.GetReferenceIds<Media, int>();
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

            loadedReferenceContext.AddReferences(
                references,
                reference => reference.Id
            );
        }

        private void LoadTags(ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext)
        {
            var lookupIds = lookupIdContext.GetReferenceIds<Tag, int>();
            var references = lookupIds
                .Select(id =>
                    new Tag
                    {
                        Id = id,
                        Name = id + "-name"
                    }
                )
                .ToList();

            loadedReferenceContext.AddReferences(
                references,
                reference => reference.Id
            );
        }

        private void LoadBlogPosts(ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext)
        {
            var lookupIds = lookupIdContext.GetReferenceIds<BlogPost, int>();
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

            loadedReferenceContext.AddReferences(
                references,
                reference => reference.Id
            );
        }

        private void LoadImages(ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext)
        {
            var lookupIds = lookupIdContext.GetReferenceIds<Image, string>();
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

            loadedReferenceContext.AddReferences(
                references,
                reference => reference.Id
            );
        }
    }
}