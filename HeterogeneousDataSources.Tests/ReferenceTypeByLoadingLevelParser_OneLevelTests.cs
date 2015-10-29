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
        [Test]
        public void ParseReferenceTypeByLoadingLevel_OneLevel()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<OneLoadingLevelContentLinkedSource>();

            var sut = TestSetupHelper.CreateReferenceTypeByLoadingLevelParser(loadLinkProtocolBuilder);
            var actual = sut.ParseLoadingLevels(typeof(OneLoadingLevelContentLinkedSource));

            Assert.That(actual,Is.Empty);
        }
    }

    public class OneLoadingLevelContentLinkedSource : ILinkedSource<OneLoadingLevelContent> {
        public OneLoadingLevelContent Model { get; set; }
    }

    public class OneLoadingLevelContent {
        public string Id { get; set; }
    }
}
