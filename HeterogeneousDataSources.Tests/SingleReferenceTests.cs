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
    public class SingleReferenceTests
    {
        private FakeReferenceLoader _referenceLoader;

        [Test]
        public void LoadLink_SingleReference()
        {
            var contentLinkedSource = SetupAndAct();

            ApprovalsExt.VerifyPublicProperties(contentLinkedSource);
        }

        [Test]
        public void LoadLink_ShouldDisposeLoader() {
            SetupAndAct();

            Assert.That(_referenceLoader.IsDisposed, Is.True);
        }

        private SingleReferenceLinkedSource SetupAndAct()
        {
            var loadLinkExpressions = new List<ILoadLinkExpression>
            {
                new LoadLinkExpression<SingleReferenceLinkedSource, Image, string>(
                    linkedSource => linkedSource.Model.SummaryImageId,
                    (linkedSource, reference) => linkedSource.SummaryImage = reference
                )
            };

            var customConfig = CreateCustomReferenceTypeConfig(
                new SingleReferenceContent{
                    Id = 1,
                    SummaryImageId = "a"
                },
                reference => reference.Id
            );
            _referenceLoader = new FakeReferenceLoader(customConfig);
            var sut = new LoadLinkProtocol(
                _referenceLoader,
                loadLinkExpressions
            );

            var contentLinkedSource = sut.LoadLink<SingleReferenceLinkedSource, int, SingleReferenceContent>(1);
            return contentLinkedSource;
        }

        public IReferenceTypeConfig CreateCustomReferenceTypeConfig<TReference, TId>(TReference fixedValue, Func<TReference, TId> getReferenceIdFunc)
        {
            return new ReferenceTypeConfig<TReference, TId>(
                ids => ids.Select(id=>fixedValue).ToList(),
                getReferenceIdFunc
            );
        }
    }


    public class SingleReferenceLinkedSource: ILinkedSource<SingleReferenceContent>
    {
        //stle: two steps loading sucks!
        public SingleReferenceContent Model { get; set; }
        public Image SummaryImage{ get; set; }
    }

    public class SingleReferenceContent {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
    }
}
