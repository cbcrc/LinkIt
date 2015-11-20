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
    public class GettingStarted {
        private ILoadLinkProtocol _loadLinkProtocol;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.ApplyConventions(
                new[]{Assembly.GetExecutingAssembly()},
                loadLinkProtocolBuilder.GetDefaultConventions()
            );

            _loadLinkProtocol = loadLinkProtocolBuilder.Build(()=>new FakeReferenceLoader());
        }

        [Test]
        public void LoadLink_ById()
        {
            var actual = _loadLinkProtocol.LoadLink<MediaLinkedSource>().ById(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_ByIds() {
            var actual = _loadLinkProtocol.LoadLink<MediaLinkedSource>().ByIds(
                new List<int>{1, 2, 3}
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }

    public class Media {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<int> TagIds { get; set; } //Tag references
    }

    public class Tag {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MediaLinkedSource : ILinkedSource<Media> {
        public Media Model { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
