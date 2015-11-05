using System;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions.DefaultConventions {
    public class LoadLinkReferenceWhenLinkedSourceModelPropertyHasIdSuffixConvention : ILoadLinkExpressionConvention {
        public bool DoesApply(
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty) 
        {
            return
                linkTargetProperty.GetLinkTargetKind() == LinkTargetKind.SingleValue &&
                linkTargetProperty.MatchLinkedSourceModelPropertyName(linkedSourceModelProperty, "Id");
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, 
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, 
            PropertyInfo linkTargetProperty, 
            PropertyInfo linkedSourceModelProperty)
        {
            loadLinkProtocolForLinkedSourceBuilder.LoadLinkReference(
                getLinkedSourceModelProperty,
                getLinkTargetProperty
            );
        }
    }
}
