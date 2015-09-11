using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.LoadLinkExpressions.Polymorphic;
using HeterogeneousDataSources.Tests.Exploratory;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests.Polymorphic {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class FactoryCanSolveGenericTests {

        private LoadLinkProtocolFactory<SingleReferenceContent, string> _loadLinkProtocolFactory;

        [SetUp]
        public void SetUp() {
            var facto = new LoadLinkExpressionFactory<SingleReferenceLinkedSource>();

            _loadLinkProtocolFactory = new LoadLinkProtocolFactory<SingleReferenceContent, string>(
                loadLinkExpressions: new List<ILoadLinkExpression>{
                    new RootLoadLinkExpression<SingleReferenceLinkedSource, SingleReferenceContent, string>(),
                    facto.CreateReferenceLoadLinkExpression(
                        linkedSource => linkedSource.Model.SummaryImageId,
                        linkedSource => linkedSource.SummaryImage
                    )
                },
                getReferenceIdFunc: reference => reference.Id
            );
        }

        [Test]
        public void LoadLink_SingleReference() {
            var sut = _loadLinkProtocolFactory.Create(
                new SingleReferenceContent {
                    Id = "1",
                    SummaryImageId = "a"
                }
            );

            var actual = sut.LoadLink<SingleReferenceLinkedSource>("1");

            ApprovalsExt.VerifyPublicProperties(actual);
        }

        [Test]
        public void X()
        {
            var linkedSourceType = typeof (PersonLinkedSource);

            var iLinkedSourceTypes = linkedSourceType.GetInterfaces()
                .Where(interfaceType =>
                    interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof (ILinkedSource<>))
                .ToList();

            //stle: throws if more than once

            var iLinkedSourceType = iLinkedSourceTypes.Single();

            var actual = iLinkedSourceType.GenericTypeArguments.Single();

            Assert.That(actual, Is.EqualTo(typeof(Person)));
        }

    }

    public class LoadLinkExpressionFactory<TLinkedSource>
    {
        public ILoadLinkExpression CreateReferenceLoadLinkExpression<TReference,TId>(
            Func<TLinkedSource, TId> getLookupIdFunc,
            Expression<Func<TLinkedSource, TReference>> linkTargetFunc) 
        {

            var setterAction = LinkTargetFactory.Create(linkTargetFunc);

            return new ReferenceLoadLinkExpression<TLinkedSource, TReference, TId>(
                getLookupIdFunc,
                setterAction.SetTargetProperty
            );
        }

    }
}
