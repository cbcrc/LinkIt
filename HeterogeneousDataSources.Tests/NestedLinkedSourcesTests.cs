using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class NestedLinkedSourcesTests
    {
        private FakeReferenceLoader<NestedContents, int> _fakeReferenceLoader;
        private LoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<NestedLinkedSources>()
                .LoadLinkNestedLinkedSource(
                    linkedSource => linkedSource.Model.AuthorIds,
                    linkedSource => linkedSource.Authors
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReference(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _fakeReferenceLoader =
                new FakeReferenceLoader<NestedContents, int>(reference => reference.Id);
            _sut = loadLinkProtocolBuilder.Build(_fakeReferenceLoader);
        }

        [Test]
        public void LoadLink_NestedLinkedSource()
        {
            _fakeReferenceLoader.FixValue(
                new NestedContents{
                    Id = 1,
                    AuthorIds = new List<string>{ "a", "b"}
                }
            );

            var actual = _sut.LoadLink<NestedLinkedSources>().ById(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_ManyReferencesWithNullInReferenceIds_ShouldLinkNull() {
            _fakeReferenceLoader.FixValue(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "a", null, "b" }
                }
            );

            var actual = _sut.LoadLink<NestedLinkedSources>().ById(1);

            Assert.That(actual.Authors.Count, Is.EqualTo(3));
            Assert.That(actual.Authors[1], Is.Null);
        }

        [Test]
        public void LoadLink_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySet() {
            _fakeReferenceLoader.FixValue(
                new NestedContents {
                    Id = 1,
                    AuthorIds = null
                }
            );

            var actual = _sut.LoadLink<NestedLinkedSources>().ById(1);

            Assert.That(actual.Authors, Is.Empty);
        }

        [Test]
        public void LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            _fakeReferenceLoader.FixValue(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "a", "a" }
                }
            );

            var actual = _sut.LoadLink<NestedLinkedSources>().ById(1);

            var linkedAuthroIds = actual.Authors.Select(author => author.Model.Id);
            Assert.That(linkedAuthroIds, Is.EquivalentTo(new[] { "a", "a" }));

            Assert.That(actual.Authors.Count, Is.EqualTo(2));
        }

        [Test]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            _fakeReferenceLoader.FixValue(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "cannot-be-resolved", "cannot-be-resolved" }
                }
            );

            var actual = _sut.LoadLink<NestedLinkedSources>().ById(1);

            Assert.That(actual.Authors, Is.EquivalentTo(new List<PersonLinkedSource>{null,null}));
        }

    }

    public class NestedLinkedSources:ILinkedSource<NestedContents>
    {
        public NestedContents Model { get; set; }
        public List<PersonLinkedSource> Authors { get; set; }
    }

    public class NestedContents {
        public int Id { get; set; }
        public List<string> AuthorIds { get; set; }
    }
}
