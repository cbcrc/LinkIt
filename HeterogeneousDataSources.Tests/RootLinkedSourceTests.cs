using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class RootLinkedSourceTests
    {
        private LoadLinkProtocolFactory<RootContent, string> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp() {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<RootContent, string>(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<RootLinkedSource, RootContent, string>(),
                },
                getReferenceIdFunc: reference => reference.Id,
                fakeReferenceTypeForLoadingLevel: new[] {
                    new List<Type>{typeof(RootContent)},
                },
                customReferenceTypeConfigs: new ReferenceTypeConfig<RootContent, string>(
                    ids => new RootContentRepository().GetByIds(ids),
                    reference => reference.Id
                )
            );
        }

        [Test]
        public void LoadLink_WithReferenceId_ShouldLinkModel() {
            var sut = _loadLinkProtocolFactory.Create();

            var actual = sut.LoadLink<RootLinkedSource>("can-be-resolved");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_WithoutReferenceId_ShouldLinkNull(){
            var sut = _loadLinkProtocolFactory.Create();

            TestDelegate act = () => sut.LoadLink<RootLinkedSource>(null);

            Assert.That(act, Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void LoadLink_CannotBeResolved_ShouldLinkNull() {
            var sut = _loadLinkProtocolFactory.Create();

            var actual = sut.LoadLink<RootLinkedSource>("cannot-be-resolved");

            Assert.That(actual, Is.Null);
        }
    }

    public class RootLinkedSource: ILinkedSource<RootContent>
    {
        public RootContent Model { get; set; }
    }

    public class RootContent {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class RootContentRepository {
        public List<RootContent> GetByIds(List<string> ids) {
            return ids
                .Where(id => id != "cannot-be-resolved")
                .Select(id => new RootContent {
                    Id = id,
                    Name = "name-"+id
                })
                .ToList();
        }
    }

}
