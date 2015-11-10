using System;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSources;
using HeterogeneousDataSources.ConfigBuilders;

namespace HeterogeneousDataSource.Conventions.Interfaces
{
    public interface ISingleValueConvention: ILoadLinkExpressionConvention
    {
        void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty,
            PropertyInfo linkTargetProperty, 
            PropertyInfo linkedSourceModelProperty
        );
    }
}