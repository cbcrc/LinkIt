using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions
{
    public interface IMultiValueConvention:ILoadLinkExpressionConvention
    {
        void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Expression<Func<TLinkedSource, List<TLinkTargetProperty>>> getLinkTargetProperty,
            Func<TLinkedSource, List<TLinkedSourceModelProperty>> getLinkedSourceModelProperty
        );
    }
}