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
    public class NestedLinkedSourcesTests
    {
        private LoadLinkProtocolFactory<NestedContents, int> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp() {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<NestedContents, int>(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<NestedLinkedSources, NestedContents, int>(),
                    new NestedLinkedSourcesLoadLinkExpression<NestedLinkedSources, PersonLinkedSource, Person, string>(
                        linkedSource => linkedSource.Model.AuthorIds,
                        (linkedSource, nestedLinkedSources) => linkedSource.Authors = nestedLinkedSources),
                    new ReferenceLoadLinkExpression<PersonLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference)
                },
                getReferenceIdFunc: reference => reference.Id,
                fakeReferenceTypeForLoadingLevel: new[] {
                        new List<Type>{typeof(NestedContents)},
                        new List<Type>{typeof(Person)},
                        new List<Type>{typeof(Image)},
                }
            );
        }

        [Test]
        public void LoadLink_NestedLinkedSource()
        {
            var sut = _loadLinkProtocolFactory.Create(
                new NestedContents{
                    Id = 1,
                    AuthorIds = new List<string>{ "a", "b"}
                }
            );

            var actual = sut.LoadLink<NestedLinkedSources>(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_ManyReferencesWithNullInReferenceIds_ShouldIgnoreNull() {
            var sut = _loadLinkProtocolFactory.Create(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "a", null, "b" }
                }
            );

            var actual = sut.LoadLink<NestedLinkedSources>(1);

            Assert.That(actual.Authors.Count, Is.EqualTo(2));
        }

        [Test]
        public void LoadLink_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySet() {
            var sut = _loadLinkProtocolFactory.Create(
                new NestedContents {
                    Id = 1,
                    AuthorIds = null
                }
            );

            var actual = sut.LoadLink<NestedLinkedSources>(1);

            Assert.That(actual.Authors, Is.Empty);
        }

        [Test]
        public void LoadLink_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            var sut = _loadLinkProtocolFactory.Create(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "a", "a" }
                }
            );

            var actual = sut.LoadLink<NestedLinkedSources>(1);

            var linkedAuthroIds = actual.Authors.Select(author => author.Model.Id);
            Assert.That(linkedAuthroIds, Is.EquivalentTo(new[] { "a", "a" }));

            Assert.That(actual.Authors.Count, Is.EqualTo(2));
        }

        [Test]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkEmptySet() {
            var sut = _loadLinkProtocolFactory.Create(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "cannot-be-resolved", "cannot-be-resolved" }
                }
            );

            var actual = sut.LoadLink<NestedLinkedSources>(1);

            Assert.That(actual.Authors, Is.Empty);
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
