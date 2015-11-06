using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSource.Conventions.Interfaces;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions.Tests.DefaultConventions
{
    public class LoadLinkMultiValueNestedLinkedSourceWhenIdSuffixMatches : IMultiValueConvention
    {
        public bool DoesApply(
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty)
        {
            if (!linkTargetProperty.IsListOfLinkedSource()) { return false; }

            return linkTargetProperty.MatchLinkedSourceModelPropertyName(linkedSourceModelProperty, "Id", "s");
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Expression<Func<TLinkedSource, List<TLinkTargetProperty>>> getLinkTargetProperty,
            Func<TLinkedSource, List<TLinkedSourceModelProperty>> getLinkedSourceModelProperty,
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty)
        {
            loadLinkProtocolForLinkedSourceBuilder.LoadLinkNestedLinkedSource(
                getLinkedSourceModelProperty,
                getLinkTargetProperty
            );
        }
    }
}