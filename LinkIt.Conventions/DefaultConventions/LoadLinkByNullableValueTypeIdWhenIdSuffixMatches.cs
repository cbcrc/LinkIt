using System;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.Core;

namespace LinkIt.Conventions.DefaultConventions {
    public class LoadLinkByNullableValueTypeIdWhenIdSuffixMatches : IByNullableValueTypeIdConvention {
        public string Name{
            get { return "Load link by nullable value type id when id suffix matches"; } 
        }

        public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty) 
        {
            return linkTargetProperty.MatchLinkedSourceModelPropertyName(linkedSourceModelProperty, "Id");
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, 
            Func<TLinkedSource, TLinkedSourceModelProperty?> getLinkedSourceModelProperty, 
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty, 
            PropertyInfo linkedSourceModelProperty, 
            PropertyInfo linkTargetProperty
        ) 
            where TLinkedSourceModelProperty : struct
        {
            if (LinkedSourceConfigs.DoesImplementILinkedSourceOnceAndOnlyOnce(typeof(TLinkTargetProperty))) {
                loadLinkProtocolForLinkedSourceBuilder.LoadLinkNestedLinkedSourceById(
                    getLinkedSourceModelProperty,
                    getLinkTargetProperty
                );
            }
            else {
                loadLinkProtocolForLinkedSourceBuilder.LoadLinkReferenceById(
                    getLinkedSourceModelProperty,
                    getLinkTargetProperty
                );
            }
        }
    }
}
