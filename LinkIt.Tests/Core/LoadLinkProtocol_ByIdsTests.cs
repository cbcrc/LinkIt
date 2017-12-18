#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests.Reporters;
using LinkIt.ConfigBuilders;
using LinkIt.PublicApi;
using LinkIt.Tests.TestHelpers;
using NUnit.Framework;


namespace LinkIt.Tests.Core
{
    public class LoadLinkConfig_ByIdsTests
    {
        private ReferenceLoaderStub _referenceLoaderStub;
        private ILoadLinkProtocol _sut;

        [SetUp]
        public void SetUp() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            loadLinkProtocolBuilder.For<PersonLinkedSource>()
                .LoadLinkReferenceById(
                    linkedSource=>linkedSource.Model.SummaryImageId,
                    linkedSource=>linkedSource.SummaryImage
                );

            _referenceLoaderStub = new ReferenceLoaderStub();
            _sut = loadLinkProtocolBuilder.Build(()=>_referenceLoaderStub);
        }


        [Fact]
        public void LoadLinkById() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById("one");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Fact]
        public void LoadLinkById_WithNullId_ShouldLinkNull() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById<string>(null);

            Assert.That(actual, Is.Null);
        }

        [Fact]
        public void LoadLinkById_WithNullId_ShouldLinkNullWithoutLoading() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById<string>(null);

            var loadedReferenceTypes = _referenceLoaderStub.RecordedLookupIdContexts
                .First()
                .GetReferenceTypes();

            Assert.That(loadedReferenceTypes, Is.Empty);
        }

        [Fact]
        public void LoadLinkById_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ById("cannot-be-resolved");

            Assert.That(actual, Is.Null);
        }

        [Fact]
        public void LoadLinkByIds() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds(new List<string>{"one","two"});

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Fact]
        public void LoadLinkByIds_WithNullInReferenceIds_ShouldLinkNull() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds(new List<string>{"one", null, "two"});

            Assert.That(
                actual.Select(personLinkSource => personLinkSource.Model.Id).ToList(),
                Is.EqualTo(new List<string> { "one", "two" })
            );
        }

        [Fact]
        public void LoadLinkByIds_WithListOfNulls_ShouldLinkNullWithoutLoading() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds<string>(new List<string>{null, null});

            Assert.That(actual, Is.Empty);
            var loadedReferenceTypes = _referenceLoaderStub.RecordedLookupIdContexts
                .First()
                .GetReferenceTypes();
            Assert.That(loadedReferenceTypes, Is.Empty);
        }


        [Fact]
        public void LoadLinkByIds_ManyReferencesWithDuplicates_ShouldLinkDuplicates() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds(new List<string>{"a", "a"});

            var linkedSourceModelIds = actual.Select(linkedSource => linkedSource.Model.Id);
            Assert.That(linkedSourceModelIds, Is.EquivalentTo(new[] { "a", "a" }));

            var loadedPersonIds = _referenceLoaderStub.RecordedLookupIdContexts
                .First()
                .GetReferenceIds<Person, string>();
            
            Assert.That(loadedPersonIds, Is.EquivalentTo(new[] { "a" }));
        }


        [Fact]
        public void LoadLinkByIds_ManyReferencesCannotBeResolved_ShouldLinkNull() {
            var actual = _sut.LoadLink<PersonLinkedSource>().ByIds(new List<string>{"cannot-be-resolved"});

            Assert.That(actual, Is.Empty);
        }
    }

}