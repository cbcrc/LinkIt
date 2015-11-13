using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ApprovalTests.Reporters;
using HeterogeneousDataSources.Tests.Shared;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.LinkedSources.Interfaces;
using NUnit.Framework;

namespace LinkIt.Conventions.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkProtocolBuilderExtensions_ConventionErrorHandlingTests {
        [Test]
        public void ApplyConventions_DoesApplyFailed_ShouldThrow(){
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
                    .With.InnerException
                        .With.Message.ContainsSubstring("does apply failed")

            );
        }

        [Test]
        public void ApplyConventions_ApplyFailed_ShouldThrow() {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            TestDelegate act = () => loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new ApplyFailedConvention() }
            );

            Assert.That(
                act,
                Throws.Exception
                    .With.Message.ContainsSubstring("ApplyFailedConvention").And
                    .With.Message.ContainsSubstring("LinkedSource/Person").And
                    .With.Message.ContainsSubstring("PersonId").And
                    .With.InnerException
                        .With.Message.ContainsSubstring("apply failed")
            );
        }


        public class DoesApplyFailedConvention: ISingleValueConvention
        {
            public string Name{
                get { return "Does apply failed convention"; } 
            }

            public bool DoesApply(
                PropertyInfo linkTargetProperty, 
                PropertyInfo linkedSourceModelProperty)
            {
                throw new Exception("does apply failed");
            }

            public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
                LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
                Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
                Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, 
                PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty)
            {}
        }

        public class ApplyFailedConvention: ISingleValueConvention {
            public string Name {
                get { return "ApplyFailedConvention"; }
            }

            public bool DoesApply(
                PropertyInfo linkTargetProperty,
                PropertyInfo linkedSourceModelProperty)
            {
                return true;
            }

            public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
                LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
                Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
                Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty,
                PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty)
            {
                throw new Exception("apply failed");
            }
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
