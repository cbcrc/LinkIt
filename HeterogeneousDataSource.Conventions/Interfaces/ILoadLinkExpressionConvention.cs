using System.Reflection;

namespace HeterogeneousDataSource.Conventions.Interfaces
{
    public interface ILoadLinkExpressionConvention
    {
        bool DoesApply(PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty);
    }
}