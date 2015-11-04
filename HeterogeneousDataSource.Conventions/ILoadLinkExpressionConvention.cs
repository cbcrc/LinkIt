using System;
using System.Reflection;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions
{
    public interface ILoadLinkExpressionConvention
    {
        string Id { get; }
        bool DoesApply(PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty);
        void Apply( LoadLinkProtocolBuilder loadLinkProtocolBuilder, Type linkedSourceType, PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty);
    }
}