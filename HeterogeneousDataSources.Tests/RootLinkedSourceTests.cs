using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class RootLinkedSourceTests
    {
        private FakeReferenceLoader<RootContent, string> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<RootLinkedSource>()
                .IsRoot<string>();

            _fakeReferenceLoader = new FakeReferenceLoader<RootContent, string>(
                reference => reference.Id,
                new ReferenceTypeConfig<RootContent, string>(
                    ids => new RootContentRepository().GetByIds(ids),
                    reference => reference.Id
                )
            );
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_WithReferenceId_ShouldLinkModel() {
            var actual = _sut.LoadLink<RootLinkedSource,string>("can-be-resolved");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_WithoutReferenceId_ShouldLinkNull(){
            TestDelegate act = () => _sut.LoadLink<RootLinkedSource, string>(null);

            Assert.That(act, Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void LoadLink_CannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<RootLinkedSource, string>("cannot-be-resolved");

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
