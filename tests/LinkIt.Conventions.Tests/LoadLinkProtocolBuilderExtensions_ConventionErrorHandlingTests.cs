// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.PublicApi;
using LinkIt.TestHelpers;
using Xunit;

namespace LinkIt.Conventions.Tests
{
    public class LoadLinkProtocolBuilderExtensions_ConventionErrorHandlingTests
    {
        [Fact]
        public void ApplyConventions_ApplyFailed_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            Action act = () => loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new ApplyFailedConvention() }
            );

            var exception = Assert.ThrowsAny<Exception>(act);
            Assert.Contains("ApplyFailedConvention", exception.Message);
            Assert.Contains("LinkedSource.Person", exception.Message);
            Assert.Contains("PersonId", exception.Message);
            Assert.Contains("apply failed", exception.InnerException.Message);
        }

        [Fact]
        public void ApplyConventions_DoesApplyFailed_ShouldThrow()
        {
            var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();

            Action act = () => loadLinkProtocolBuilder.ApplyConventions(
                new List<Type> { typeof(LinkedSource) },
                new List<ILoadLinkExpressionConvention> { new DoesApplyFailedConvention() }
            );

            var exception = Assert.ThrowsAny<Exception>(act);
            Assert.Contains("Does apply failed convention", exception.Message);
            Assert.Contains("LinkedSource.Person", exception.Message);
            Assert.Contains("PersonId", exception.Message);
            Assert.Contains("does apply failed", exception.InnerException.Message);
        }

        public class DoesApplyFailedConvention : IReferenceConvention
        {
            public string Name => "Does apply failed convention";

            public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
            {
                throw new Exception("does apply failed");
            }

            public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty, PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
                where TLinkedSource: ILinkedSource
            {
            }
        }

        public class ApplyFailedConvention : IReferenceConvention
        {
            public string Name => "ApplyFailedConvention";

            public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
            {
                return true;
            }

            public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty, PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
                where TLinkedSource: ILinkedSource
            {
                throw new Exception("apply failed");
            }
        }

        public class LinkedSource : ILinkedSource<Model>
        {
            public Person Person { get; set; }
            public Model Model { get; set; }
        }

        public class Model
        {
            public string PersonId { get; set; }
        }
    }
}