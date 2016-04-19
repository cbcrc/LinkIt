#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Reflection;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions;
using LinkIt.Conventions.DefaultConventions;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;


namespace LinkIt.Samples {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class GettingStarted {
        private ILoadLinkProtocol _loadLinkProtocol;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            _loadLinkProtocol = loadLinkProtocolBuilder.Build(
                ()=>new FakeReferenceLoader(),
                new[] { Assembly.GetExecutingAssembly() },
                LoadLinkExpressionConvention.Default
            );
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
