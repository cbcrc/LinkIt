using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;

namespace LinkIt.Conventions.DefaultConventions {
    public class LoadLinkMultiValueNestedLinkedSourceFromModelWhenNameMatches : IMultiValueConvention {
        public string Name {
            get { return "Load link multi value nested linked source from model when name matches"; }
        }
        
        public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty) 
        {
            return linkTargetProperty.Name == linkedSourceModelProperty.Name;
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, 
            Func<TLinkedSource, List<TLinkedSourceModelProperty>> getLinkedSourceModelProperty, 
            Expression<Func<TLinkedSource, List<TLinkTargetProperty>>> getLinkTargetProperty, 
            PropertyInfo linkedSourceModelProperty, 
            PropertyInfo linkTargetProperty)
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
