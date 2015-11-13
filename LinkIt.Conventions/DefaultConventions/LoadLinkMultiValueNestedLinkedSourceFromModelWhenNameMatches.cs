using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSources.ConfigBuilders;
using LinkIt.Conventions.Interfaces;

namespace LinkIt.Conventions.DefaultConventions {
    public class LoadLinkMultiValueNestedLinkedSourceFromModelWhenNameMatches : IMultiValueConvention {
        public string Name {
            get { return "Load link multi value nested linked source from model when name matches"; }
        }
        
        public bool DoesApply(
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty) 
        {
            return linkTargetProperty.Name == linkedSourceModelProperty.Name;
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, 
            Expression<Func<TLinkedSource, List<TLinkTargetProperty>>> getLinkTargetProperty,
            Func<TLinkedSource, List<TLinkedSourceModelProperty>> getLinkedSourceModelProperty, 
            PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty)
        {
            loadLinkProtocolForLinkedSourceBuilder.PolymorphicLoadLinkForList(
                getLinkedSourceModelProperty,
                getLinkTargetProperty,
                link => true,
                includes => includes
                    .Include<TLinkTargetProperty>().AsNestedLinkedSourceFromModel(
                        true,
                        link => link
                    )
            );
        }
    }
}
