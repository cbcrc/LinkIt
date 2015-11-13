using System;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSources.ConfigBuilders;

namespace LinkIt.Conventions.Interfaces
{
    public interface IByNullableValueTypeIdConvention : ILoadLinkExpressionConvention
    {
        void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            Func<TLinkedSource, TLinkedSourceModelProperty?> getLinkedSourceModelProperty,
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty
        ) where TLinkedSourceModelProperty:struct;
    }
}