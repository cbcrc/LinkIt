using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using ApprovalTests.Reporters;
using HeterogeneousDataSource.Conventions.Interfaces;
using HeterogeneousDataSources;
using HeterogeneousDataSources.Tests.Shared;
using NUnit.Framework;

namespace HeterogeneousDataSource.Conventions.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkProtocolBuilderExtensions_ConventionErrorHandlingTests {
        [Test]
        public void ApplyConventions_DoesApplyFailed_ShouldMatchExpectedLinkTargets(){
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
            
            TestDelegate act = () => loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new DoesApplyFailedConvention() }
            );

            Assert.That(
                act,
                Throws.Exception
                    .With.Message.ContainsSubstring("Does apply failed convention").And
                    .With.Message.ContainsSubstring("LinkedSource/Person").And
                    .With.Message.ContainsSubstring("PersonId").And
                    .With.InnerException.Not.Null
            );
        }

        public class DoesApplyFailedConvention:ISingleValueConvention
        {
            public string Name{
                get { return "Does apply failed convention"; } 
            }

            public bool DoesApply(
                PropertyInfo linkTargetProperty, 
                PropertyInfo linkedSourceModelProperty)
            {
                throw new Exception("Does apply failed");
            }

            public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
                LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
                Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
                Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, 
                PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty)
            {}
        }

        public class LinkedSource : ILinkedSource<Model> {
            public Model Model { get; set; }
            public Person Person { get; set; }
        }

        public class Model {
            public string PersonId { get; set; }
        }
    }
}
