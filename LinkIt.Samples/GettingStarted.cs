using System.Collections.Generic;
using System.Reflection;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions;
using LinkIt.LinkedSources.Interfaces;
using LinkIt.Protocols;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Samples {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class GettingStarted {
        private LoadLinkProtocol _loadLinkProtocol;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.ApplyConventions(
                new[]{Assembly.GetExecutingAssembly()},
                loadLinkProtocolBuilder.GetDefaultConventions()
            );

            _loadLinkProtocol = loadLinkProtocolBuilder.Build(new FakeReferenceLoader());
        }

        [Test]
        public void LoadLink_ById()
        {
            var actual = _loadLinkProtocol.LoadLink<MediaLinkedSource>().ById("one");

            //stle: what to do?
            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_ByIds() {
            var actual = _loadLinkProtocol.LoadLink<MediaLinkedSource>().ByIds("one", "two", "three");

            //stle: what to do?
            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_Query()
        {
            var result = GetMediaByKeyword("fish");

            var actual = _loadLinkProtocol.LoadLink<MediaLinkedSource>().FromModels(result);

            //stle: what to do?
            ApprovalsExt.VerifyPublicProperties(actual);
        }

        //stle: should support enumerable in from models
        private Media[] GetMediaByKeyword(string fish)
        {
            //fake result of a database query
            return new []{
                new Media{
                    Id = "99",
                    Title = "Humpback Whale",
                    TagIds = new List<string> {"47"}
                },
                new Media{
                    Id = "74",
                    Title = "Manta Ray",
                    TagIds = new List<string> {}
                }
            };
        }
    }

    public class Media {
        public string Id { get; set; }
        public string Title { get; set; }
        //stle: set this as a IEnumerable
        public List<string> TagIds { get; set; }
    }

    public class Tag {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class MediaLinkedSource : ILinkedSource<Media> {
        public Media Model { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
