using System.Collections.Generic;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class MultipleReferencesTypeTests
    {
        private LoadLinkProtocolFactory<MultipleReferencesTypeContent, int> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp() {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<MultipleReferencesTypeContent, int>(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<MultipleReferencesTypeLinkedSource, MultipleReferencesTypeContent, int>(),
                    new ReferenceLoadLinkExpression<MultipleReferencesTypeLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference
                    ),
                    new ReferenceLoadLinkExpression<MultipleReferencesTypeLinkedSource,Person, string>(
                        linkedSource => linkedSource.Model.AuthorId,
                        (linkedSource, reference) => linkedSource.Author = reference
                    )
                },
                getReferenceIdFunc: reference => reference.Id
            );
        }

        [Test]
        public void LoadLink_MultipleReferencesTypeTests()
        {
            var sut = _loadLinkProtocolFactory.Create(
                new MultipleReferencesTypeContent()
                {
                    Id = 1,
                    SummaryImageId = "a",
                    AuthorId = "32"
                }
            );

            var actual = sut.LoadLink<MultipleReferencesTypeLinkedSource>(1);

            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }


    public class MultipleReferencesTypeLinkedSource : ILinkedSource<MultipleReferencesTypeContent>
    {
        public MultipleReferencesTypeContent Model { get; set; }
        public Image SummaryImage { get; set; }
        public Person Author { get; set; }
    }

    public class MultipleReferencesTypeContent {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
        public string AuthorId { get; set; }
    }
}
