using System;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSource.Conventions.Interfaces;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions.DefaultConventions {
    public class LoadLinkSingleValueSubLinkedSourceWhenNameMatches : ISingleValueConvention {
        public bool DoesApply(
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty) 
        {
            return linkTargetProperty.Name == linkedSourceModelProperty.Name;
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, 
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty,
            PropertyInfo linkTargetProperty, 
            PropertyInfo linkedSourceModelProperty)
        {
            loadLinkProtocolForLinkedSourceBuilder.PolymorphicLoadLink(
                getLinkedSourceModelProperty,
                getLinkTargetProperty,
                link => true, 
                includes => includes
                    .Include<TLinkTargetProperty>().AsSubLinkedSource(
                        true,
                        link=>link                        
                    )
            );
        }
    }
}
