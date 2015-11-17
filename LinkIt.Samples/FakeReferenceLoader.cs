using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Protocols;
using LinkIt.Protocols.Interfaces;

namespace LinkIt.Samples {
    
    //stle: rename the other FakeReferenceLoader to ReferenceLoaderStub
    public class FakeReferenceLoader:IReferenceLoader
    {
        public void LoadReferences(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext){
            foreach (var referenceType in lookupIdContext.GetReferenceTypes()){
                LoadReference(referenceType, lookupIdContext, loadedReferenceContext);
            }
        }

        private void LoadReference(Type referenceType, LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext)
        {
            if (referenceType == typeof(Tag)){
                LoadTags(lookupIdContext, loadedReferenceContext);
            }
            if (referenceType == typeof(Media)) {
                LoadMedia(lookupIdContext, loadedReferenceContext); 
            }
            if (referenceType == typeof(Image)) {
                LoadImages(lookupIdContext, loadedReferenceContext);
            }
            if (referenceType == typeof(BlogPost)) {
                LoadBlogPosts(lookupIdContext, loadedReferenceContext);
            }
        }

        private void LoadMedia(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext) {
            var lookupIds = lookupIdContext.GetReferenceIds<Media, string>();
            var references = lookupIds
                .Select(id =>
                    new Media{
                        Id = id,
                        Title = "title-" + id,
                        TagIds = new List<string> { 
                            string.Format("tag-{0}-a", id),
                            string.Format("tag-{0}-b", id)
                        }
                    }
                )
                .ToList();

            loadedReferenceContext.AddReferences(
                references,
                reference => reference.Id
            );
        }

        private void LoadTags(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext){
            var lookupIds = lookupIdContext.GetReferenceIds<Tag, string>();
            var references = lookupIds
                .Select(id=>
                    new Tag {
                        Id = id, 
                        Name = id+"-name"
                    }
                )
                .ToList();
            
            loadedReferenceContext.AddReferences(
                references,
                reference => reference.Id
            );
        }

        private void LoadBlogPosts(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext) {
            var lookupIds = lookupIdContext.GetReferenceIds<BlogPost, int>();
            var references = lookupIds
                .Select(id =>
                    new BlogPost {
                        Id = id,
                        Author = new Author{
                            Name = "author-name-" +id,
                            Email = "author-email-" +id,
                            ImageId = "author-image-" +id,
                        },
                        MultimediaContentRef = new MultimediaContentReference{
                            Type = id % 2 == 0
                                ? "image"
                                : "media",
                            Id = "multi-"+id
                        },
                        TagIds = new List<string>{
                            "fake-blog-post-tag-"+(100+id),
                            "fake-blog-post-tag-"+(101+id)
                        },
                        Title = "Title-"+id
                    }
                )
                .ToList();

            loadedReferenceContext.AddReferences(
                references,
                reference => reference.Id
            );
        }

        private void LoadImages(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext) {
            var lookupIds = lookupIdContext.GetReferenceIds<Image, string>();
            var references = lookupIds
                .Select(id =>
                    new Image {
                        Id = id,
                        Alt = id + "-alt",
                        Url = id + "-url"
                    }
                )
                .ToList();

            loadedReferenceContext.AddReferences(
                references,
                reference => reference.Id
            );
        }

        public void Dispose(){
            //in case you need to dispose database connections or other ressources.
        }

        public object GetOpenedConnection(string connectionName)
        {
            //in case you need to share opened connection or other ressources.
            return connectionName; //a real implementation would return an opened connection
        }
    }
}
