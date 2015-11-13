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
                    .Include<TLinkTargetProperty>().AsNestedLinkedSourceFromModel(
                        true,
                        link=>link                        
                    )
            );
        }
    }
}
