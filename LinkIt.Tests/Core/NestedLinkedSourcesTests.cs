using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace LinkIt.Tests.Core {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class NestedLinkedSourcesTests
    {
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<NestedLinkedSources>()
                .LoadLinkNestedLinkedSourceById(
                    linkedSource => linkedSource.Model.AuthorIds,
                    linkedSource => linkedSource.Authors
                );
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    linkedSource => linkedSource.SummaryImage
                );

            _sut = loadLinkProtocolBuilder.Build(() => new ReferenceLoaderStub());
        }

        [Test]
        public void LoadLink_NestedLinkedSource()
        {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "a", "b" }
                }
            );

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_ManyReferencesWithNullInReferenceIds_ShouldLinkNull() {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "a", null, "b" }
                }
            );

            Assert.That(actual.Authors.Count, Is.EqualTo(3));
            Assert.That(actual.Authors[1], Is.Null);
        }

        [Test]
        public void LoadLink_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySet() {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents {
                    Id = 1,
                    AuthorIds = null
                }
            );

            Assert.That(actual.Authors, Is.Empty);
        }

        [Test]
        public void LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "a", "a" }
                }
            );

            var linkedAuthroIds = actual.Authors.Select(author => author.Model.Id);
            Assert.That(linkedAuthroIds, Is.EquivalentTo(new[] { "a", "a" }));

            Assert.That(actual.Authors.Count, Is.EqualTo(2));
        }

        [Test]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "cannot-be-resolved", "cannot-be-resolved" }
                }
            );

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
