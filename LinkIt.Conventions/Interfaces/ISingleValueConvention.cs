using System;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;

namespace LinkIt.Conventions.Interfaces
{
    public interface ISingleValueConvention: ILoadLinkExpressionConvention
    {
        void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty, 
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty, 
            PropertyInfo linkedSourceModelProperty, 
            PropertyInfo linkTargetProperty
        );
    }
}