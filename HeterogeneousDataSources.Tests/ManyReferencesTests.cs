using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class ManyReferencesTests
    {
        private LoadLinkProtocolFactory<ManyReferencesContent, int> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp() {
            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<ManyReferencesContent, int>(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new ReferenceLoadLinkExpression<ManyReferencesLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        (linkedSource, reference) => linkedSource.SummaryImage = reference
                    ),
                    new ReferenceLoadLinkExpression<ManyReferencesLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.AuthorImageId,
                        (linkedSource, reference) => linkedSource.AuthorImage = reference
                    ),
                    new ReferencesLoadLinkExpression<ManyReferencesLinkedSource,Image, string>(
                        linkedSource => linkedSource.Model.FavoriteImageIds,
                        (linkedSource, reference) => linkedSource.FavoriteImages = reference
                    )
                },
                getReferenceIdFunc: reference => reference.Id,
                fakeReferenceTypeForLoadingLevel: new[] {
                    new List<Type>{typeof(ManyReferencesContent)},
                    new List<Type>{typeof(Image)},
                }
            );
        }

        [Test]
        public void LoadLink_ManyReferences()
        {
            var sut = _loadLinkProtocolFactory.Create(
                new ManyReferencesContent {
                    Id = 1,
                    SummaryImageId = "summary-image-id",
                    AuthorImageId = "author-image-id",
                    FavoriteImageIds = new List<string>{"one","two"}
                }
            );

            var actual = sut.LoadLink<ManyReferencesLinkedSource,int,ManyReferencesContent>(1);
            
            ApprovalsExt.VerifyPublicProperties(actual);
        }
    }

    public class ManyReferencesLinkedSource: ILinkedSource<ManyReferencesContent>{
        public ManyReferencesContent Model { get; set; }
        public Image SummaryImage { get; set; }
        public Image AuthorImage { get; set; }
        public List<Image> FavoriteImages { get; set; }
    }

    public class ManyReferencesContent {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
        public string AuthorImageId { get; set; }
        public List<string> FavoriteImageIds { get; set; }
    }

}