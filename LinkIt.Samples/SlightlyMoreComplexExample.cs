using System.Collections.Generic;
using System.Reflection;
using ApprovalTests.Reporters;
using HeterogeneousDataSource.Conventions;
using HeterogeneousDataSources.ConfigBuilders;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Samples {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class SlightlyMoreComplexExample {
        private LoadLinkProtocol _loadLinkProtocol;

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

            _loadLinkProtocol = loadLinkProtocolBuilder.Build(new FakeReferenceLoader());
        }

        [Test]
        public void LoadLinkById_WithImage()
        {
            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().ById(1);

            //stle: what to do?
            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLinkById_WithMedia() {
            var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().ById(2);

            //stle: what to do?
            ApprovalsExt.VerifyPublicProperties(actual);
        }

    }

    public class BlogPost {
        public int Id { get; set; }
        public string Title { get; set; }
        //stle: set this as a IEnumerable
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
