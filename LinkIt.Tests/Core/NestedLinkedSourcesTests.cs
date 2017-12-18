#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;


namespace LinkIt.Tests.Core {
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

        [Fact]
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

        [Fact]
        public void LoadLink_ManyReferencesWithNullInReferenceIds_ShouldLinkNull() {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "a", null, "b" }
                }
            );

            Assert.That(
                actual.Authors.Select(author=>author.Model.Id).ToList(), 
                Is.EqualTo(new List<string>{"a","b"})
            );
        }

        [Fact]
        public void LoadLink_ManyReferencesWithoutReferenceIds_ShouldLinkEmptySet() {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents {
                    Id = 1,
                    AuthorIds = null
                }
            );

            Assert.That(actual.Authors, Is.Empty);
        }

        [Fact]
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

        [Fact]
        public void LoadLink_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<NestedLinkedSources>().FromModel(
                new NestedContents {
                    Id = 1,
                    AuthorIds = new List<string> { "cannot-be-resolved", "cannot-be-resolved" }
                }
            );

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
