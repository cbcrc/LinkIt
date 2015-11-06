using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions.DefaultConventions {
    public class LoadLinkMultiValueReferencesWhenIdSuffixMatchesConvention : IMultiValueConvention {
        public bool DoesApply(
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty) 
        {
            return linkTargetProperty.MatchLinkedSourceModelPropertyName(linkedSourceModelProperty, "Id", "s");
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, 
            Expression<Func<TLinkedSource, List<TLinkTargetProperty>>> getLinkTargetProperty,
            Func<TLinkedSource, List<TLinkedSourceModelProperty>> getLinkedSourceModelProperty)
        {
            loadLinkProtocolForLinkedSourceBuilder.LoadLinkReference(
                getLinkedSourceModelProperty,
                getLinkTargetProperty
            );
        }
    }
}
