using System;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions.DefaultConventions {
    public class LoadLinkReferencesWhenLinkedSourceModelPropertyHasIdSuffixConvention : ILoadLinkExpressionConvention {
        public bool DoesApply(
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty) 
        {
            return
                linkTargetProperty.GetLinkTargetKind() == LinkTargetKind.MultiValue &&
                linkTargetProperty.MatchLinkedSourceModelPropertyName(linkedSourceModelProperty, "Id", "s");
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, 
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, 
            PropertyInfo linkTargetProperty, 
            PropertyInfo linkedSourceModelProperty)
        {
//stle: oups: must have tree type of convention

            //loadLinkProtocolForLinkedSourceBuilder.LoadLinkReferences(
            //    getLinkedSourceModelProperty,
            //    getLinkTargetProperty
            //);
        }
    }
}
