using System;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSource.Conventions.Interfaces;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions.Tests.DefaultConventions
{
    public class LoadLinkNestedLinkedSourceByNullableValueTypeIdWhenIdSuffixMatches : IByNullableValueTypeIdConvention {
        public bool DoesApply(
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty) 
        {
            if (!linkTargetProperty.IsLinkedSource()) { return false; }

            return linkTargetProperty.MatchLinkedSourceModelPropertyName(linkedSourceModelProperty, "Id");
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            Func<TLinkedSource, TLinkedSourceModelProperty?> getLinkedSourceModelProperty,
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty
        )
            where TLinkedSourceModelProperty : struct 
        {
            loadLinkProtocolForLinkedSourceBuilder.LoadLinkNestedLinkedSource(
                getLinkedSourceModelProperty,
                getLinkTargetProperty
            );
        }
    }
}