using System;
using System.Linq.Expressions;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions
{
    public interface ISingleValueConvention: ILoadLinkExpressionConvention
    {
        void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty
        );
    }
}