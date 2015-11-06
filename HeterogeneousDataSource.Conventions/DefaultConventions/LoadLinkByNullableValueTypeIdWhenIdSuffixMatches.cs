using System;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSource.Conventions.Interfaces;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions.DefaultConventions {
    public class LoadLinkByNullableValueTypeIdWhenIdSuffixMatches : IByNullableValueTypeIdConvention {
        public string Name{
            get { return "Load link by nullable value type id when id suffix matches"; } 
        }

        public bool DoesApply(
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty) 
        {
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
            if (LinkedSourceConfigs.DoesImplementILinkedSourceOnceAndOnlyOnce(typeof(TLinkTargetProperty))) {
                loadLinkProtocolForLinkedSourceBuilder.LoadLinkNestedLinkedSource(
                    getLinkedSourceModelProperty,
                    getLinkTargetProperty
                );
            }
            else {
                loadLinkProtocolForLinkedSourceBuilder.LoadLinkReference(
                    getLinkedSourceModelProperty,
                    getLinkTargetProperty
                );
            }
        }
    }
}
