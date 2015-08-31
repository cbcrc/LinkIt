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
    public class NestedLinkedSourceTests
    {
        private LoadLinkProtocolFactory<NestedContent, int> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp() {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<NestedContent, int>(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<NestedLinkedSource, NestedContent, int>(),
                    new NestedLinkedSourceLoadLinkExpression<NestedLinkedSource, PersonLinkedSource, Person, string>(
                        linkedSource => linkedSource.Model.AuthorDetailId,
                        (linkedSource, nestedLinkedSource) => linkedSource.AuthorDetail = nestedLinkedSource),
                    new ReferenceLoadLinkExpression<NestedLinkedSource,Person, string>(
                        linkedSource => linkedSource.Model.ClientSummaryId,
                        (linkedSource, reference) => linkedSource.ClientSummary = reference),
                    new ReferenceLoadLinkExpression<PersonLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference)
                },
                getReferenceIdFunc: reference => reference.Id,
                fakeReferenceTypeForLoadingLevel: new[] {
                        new List<Type>{typeof(NestedContent)},
                        new List<Type>{typeof(Person)},
                        new List<Type>{typeof(Image)},
                }
            );
        }

        [Test]
        public void LoadLink_NestedLinkedSource()
        {
            var sut = _loadLinkProtocolFactory.Create(
                new NestedContent{
                    Id = 1,
                    AuthorDetailId = "32",
                    ClientSummaryId = "33"
                }
            );

            var actual = sut.LoadLink<NestedLinkedSource>(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void LoadLink_DifferendKindOfPersonInSameRootLinkedSource_ShouldNotLoadImageFromClientSummary() {
            var sut = _loadLinkProtocolFactory.Create(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "32",
                    ClientSummaryId = "666" //Image repository throws an exception for "person-img-666" 
                }
            );

            sut.LoadLink<NestedLinkedSource>(1);

            //stle: improve this by allowing test visibility on which image id was resolved
            //assert that does not throw
        }

        [Test]
        public void LoadLink_NestedLinkedSourceWithoutReferenceId_ShouldLinkNull() {
            var sut = _loadLinkProtocolFactory.Create(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = null,
                    ClientSummaryId = "33"
                }
            );

            var actual = sut.LoadLink<NestedLinkedSource>(1);

            Assert.That(actual.AuthorDetail, Is.Null);
        }

        [Test]
        public void LoadLink_NestedLinkedSourceCannotBeResolved_ShouldLinkNull() {
            var sut = _loadLinkProtocolFactory.Create(
                new NestedContent {
                    Id = 1,
                    AuthorDetailId = "cannot-be-resolved",
                    ClientSummaryId = "33"
                }
            );

            var actual = sut.LoadLink<NestedLinkedSource>(1);

            Assert.That(actual.AuthorDetail, Is.Null);
        }

        [Test]
        public void LoadLink_NestedLinkedSourceRootCannotBeResolved_ShouldReturnNullAsRoot() {
            var sut = _loadLinkProtocolFactory.Create(null);

            var actual = sut.LoadLink<NestedLinkedSource>(1);

            Assert.That(actual, Is.Null);
        }
    }


    public class NestedLinkedSource:ILinkedSource<NestedContent>
    {
        public NestedContent Model { get; set; }
        public PersonLinkedSource AuthorDetail { get; set; }
        public Person ClientSummary { get; set; }
    }

    public class NestedContent {
        public int Id { get; set; }
        public string AuthorDetailId { get; set; }
        public string ClientSummaryId { get; set; }
    }

    public class PersonLinkedSource: ILinkedSource<Person>
    {
        public Person Model { get; set; }
        public Image SummaryImage{ get; set; }
    }
}
