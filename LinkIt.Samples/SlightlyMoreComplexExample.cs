using System.Collections.Generic;
using System.Reflection;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions;
using LinkIt.Conventions.DefaultConventions;
using LinkIt.PublicApi;
using LinkIt.Shared;
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
            _loadLinkProtocol = loadLinkProtocolBuilder.Build(
                () => new FakeReferenceLoader(),
                Assembly.GetExecutingAssembly().Yield(),
                LoadLinkExpressionConvention.Default
            );
        }

        [Test]
        public void LoadLinkById()
        {
            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().ById(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLinkByIds() {
            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().ByIds(
                new List<int>{3,2,1}
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_FromQuery(){
            var models = GetBlogPostByKeyword("fish");
            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().FromModels(models);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_FromTransiantModel() {
            var model = new BlogPost {
                Id = 101,
                Author = new Author{
                    Name = "author-name-101",
                    Email = "author-email-101",
                    ImageId = "distinc-id-loaded-once", //same entity referenced twice
                },
                MultimediaContentRef = new MultimediaContentReference{
                    Type = "image",
                    Id = "distinc-id-loaded-once" //same entity referenced twice
                },
                TagIds = new List<int>{
                    1001,
                    1002
                },
                Title = "Title-101"
            };
            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().FromModel(model);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        private List<BlogPost> GetBlogPostByKeyword(string fish) {
            //fake result of a database query
            return new List<BlogPost>{
                new BlogPost {
                    Id = 77,
                    Author = new Author{
                        Name = "author-name-77",
                        Email = "author-email-77",
                        ImageId = "id-77",
                    },
                    MultimediaContentRef = new MultimediaContentReference{
                        Type = "media",
                        Id = 277
                    },
                    TagIds = new List<int>{
                        177,
                        178
                    },
                    Title = "Salmon"
                },
                new BlogPost {
                    Id = 78,
                    Author = new Author{
                        Name = "author-name-78",
                        Email = "author-email-78",
                        ImageId = "id-78",
                    },
                    MultimediaContentRef = new MultimediaContentReference{
                        Type = "image",
                        Id = "id-123"
                    },
                    TagIds = new List<int>{
                        178
                    },
                    Title = "Cod"
                }
            };
        }
    }

    public class BlogPost {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<int> TagIds { get; set; }
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
        public object Id { get; set; }
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

    public class BlogPostLinkedSourceConfig : ILoadLinkProtocolConfig
    {
        public void ConfigureLoadLinkProtocol(LoadLinkProtocolBuilder loadLinkProtocolBuilder)
        {
            loadLinkProtocolBuilder.For<BlogPostLinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.MultimediaContentRef,
                    linkedSource => linkedSource.MultimediaContent,
                    link => link.Type,
                    includes => includes
                        .Include<MediaLinkedSource>().AsNestedLinkedSourceById(
                            "media",
                            link => (int)link.Id
                        )
                        .Include<Image>().AsReferenceById(
                            "image",
                            link => (string)link.Id
                        )
                );
        }
    }
}
