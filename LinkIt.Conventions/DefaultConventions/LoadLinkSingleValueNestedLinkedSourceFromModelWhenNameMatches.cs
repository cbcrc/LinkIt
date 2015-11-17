using System;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;

namespace LinkIt.Conventions.DefaultConventions {
    public class LoadLinkSingleValueNestedLinkedSourceFromModelWhenNameMatches : ISingleValueConvention {
        public string Name {
            get { return "Load link single value nested linked source from model source when name matches"; }
        }

        public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty) 
        {
            return linkTargetProperty.Name == linkedSourceModelProperty.Name;
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, 
            Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, 
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty, 
            PropertyInfo linkedSourceModelProperty, 
            PropertyInfo linkTargetProperty)
        {
            loadLinkProtocolForLinkedSourceBuilder.PolymorphicLoadLink(
                getLinkedSourceModelProperty,
                getLinkTargetProperty,
                link => true, 
                includes => includes
                    .Include<TLinkTargetProperty>().AsNestedLinkedSourceFromModel(
                        true,
                        link=>link                        
                    )
            );
        }
    }
}
