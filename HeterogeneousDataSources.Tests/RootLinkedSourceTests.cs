using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.ConfigBuilders;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.Protocols;
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
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.ImageId,
                    linkedSource => linkedSource.Image
                );

            _fakeReferenceLoader = new FakeReferenceLoader<RootContent, string>(
                reference => reference.Id,
                new ReferenceTypeConfig<RootContent, string>(
                    ids => new RootContentRepository().GetByIds(ids),
                    reference => reference.Id
                )
            );
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        //stle: dry test?
        [Test]
        public void LoadLink_WithReferenceId_ShouldLinkModel() {
            var actual = _sut.LoadLink<RootLinkedSource>().ById("can-be-resolved");

            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void LoadLink_CannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<RootLinkedSource>().ById("cannot-be-resolved");

            Assert.That(actual, Is.Null);
        }
    }

    public class RootLinkedSource: ILinkedSource<RootContent>
    {
        public RootContent Model { get; set; }
        public Image Image { get; set; }
    }

    public class RootContent {
        public string Id { get; set; }
        public string ImageId { get; set; }
    }

    public class RootContentRepository {
        public List<RootContent> GetByIds(List<string> ids) {
            return ids
                .Where(id => id != "cannot-be-resolved")
                .Select(id => new RootContent {
                    Id = id,
                    ImageId = "name-"+id
                })
                .ToList();
        }
    }

}
