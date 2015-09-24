using System;
using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ReferenceTypeByLoadingLevelParser_OneLevelTests
    {
        private FakeReferenceLoader<NestedContents, int> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        private ReferenceTypeByLoadingLevelParser CreateSut(List<ILoadLinkExpression> loadLinkExpressions)
        {
            var factory = new LoadLinkExpressionTreeFactory(loadLinkExpressions);
            return new ReferenceTypeByLoadingLevelParser(factory);
        }

        [Test]
        public void GetReferenceTypeForLoadingLevel_OneLevel()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<OneLoadingLevelContentLinkedSource>()
                .IsRoot<string>();
            var rootLoadLinkExpression = loadLinkProtocolBuilder.GetLoadLinkExpressions()[0];
            var sut = CreateSut(loadLinkProtocolBuilder.GetLoadLinkExpressions());

            var actual = sut.ParseReferenceTypeByLoadingLevel(rootLoadLinkExpression);

            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }

    public class OneLoadingLevelContentLinkedSource : ILinkedSource<OneLoadingLevelContent> {
        public OneLoadingLevelContent Model { get; set; }
    }

    public class OneLoadingLevelContent {
        public string Id { get; set; }
    }
}
