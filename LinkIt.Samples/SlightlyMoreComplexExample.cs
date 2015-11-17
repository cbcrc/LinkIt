using System.Collections.Generic;
using System.Reflection;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions;
using LinkIt.PublicApi;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Samples {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class SlightlyMoreComplexExample {
        private ILoadLinkProtocol _loadLinkProtocol;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.ApplyConventions(
                new[]{Assembly.GetExecutingAssembly()},
                loadLinkProtocolBuilder.GetDefaultConventions()
            );
            loadLinkProtocolBuilder.For<BlogPostLinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.MultimediaContentRef,
                    linkedSource => linkedSource.MultimediaContent,
                    link => link.Type,
                    includes => includes
                        .Include<Image>().AsReferenceById(
                            "image",
                            link => link.Id
                        )
                        .Include<MediaLinkedSource>().AsNestedLinkedSourceById(
                            "media",
                            link => link.Id
                        )
                );

            _loadLinkProtocol = loadLinkProtocolBuilder.Build(()=>new FakeReferenceLoader());
        }

        [Test]
        public void LoadLinkById()
        {
            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().ById(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLinkByIds() {
            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().ByIds(2,1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_FromQuery() {
            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().FromQuery(
                () => GetBlogPostByKeyword("fish")
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_FromQueryWithDependencies() {
            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().FromQuery(
                referenceLoader => {
                    var referenceLoaderImpl = (FakeReferenceLoader)referenceLoader;
                    var fakeConnection = referenceLoaderImpl.GetOpenedConnection("MyConnectionName");
                    return GetBlogPostByKeyword("fish", fakeConnection);
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }


        [Test]
        public void LoadLink_FromTransiantModel() {
            var model = new BlogPost {
                Id = 77,
                Author = new Author{
                    Name = "author-name-77",
                    Email = "author-email-77",
                    ImageId = "author-image-77",
                },
                MultimediaContentRef = new MultimediaContentReference{
                    Type = "media",
                    Id = "multi-77"
                },
                TagIds = new List<string>{
                    "fake-blog-post-tag-"+(177),
                    "fake-blog-post-tag-"+(178)
                },
                Title = "Title-77"
            };

            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().FromModel(model);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        private List<BlogPost> GetBlogPostByKeyword(string fish, object fakeConnection = null) {
            //fake result of a database query
            return new List<BlogPost>{
                new BlogPost {
                    Id = 77,
                    Author = new Author{
                        Name = "author-name-77",
                        Email = "author-email-77",
                        ImageId = "author-image-77",
                    },
                    MultimediaContentRef = new MultimediaContentReference{
                        Type = "media",
                        Id = "multi-77"
                    },
                    TagIds = new List<string>{
                        "fake-blog-post-tag-"+(177),
                        "fake-blog-post-tag-"+(178)
                    },
                    Title = "Title-77"
                },
                new BlogPost {
                    Id = 101,
                    Author = new Author{
                        Name = "author-name-101",
                        Email = "author-email-101",
                        ImageId = "author-image-101",
                    },
                    MultimediaContentRef = new MultimediaContentReference{
                        Type = "media",
                        Id = "multi-101"
                    },
                    TagIds = new List<string>{
                        "fake-blog-post-tag-"+(201),
                        "fake-blog-post-tag-"+(202)
                    },
                    Title = "Title-101"
                },
            };
        }
    }

    public class BlogPost {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<string> TagIds { get; set; }
        public Author Author { get; set; }
        public MultimediaContentReference MultimediaContentRef { get; set; }
    }

    public class Author {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImageId { get; set; }
    }

    public class MultimediaContentReference {
        public string Type { get; set; }
        public string Id { get; set; }
    }

    public class Image {
        public string Id { get; set; }
        public string Alt { get; set; }
        public string Url { get; set; }
    }

    public class BlogPostLinkedSource : ILinkedSource<BlogPost> {
        public BlogPost Model { get; set; }
        public List<Tag> Tags { get; set; }
        public AuthorLinkedSource Author { get; set; }
        public object MultimediaContent { get; set; }
    }

    public class AuthorLinkedSource : ILinkedSource<Author> {
        public Author Model { get; set; }
        public Image Image { get; set; }
    }
}
