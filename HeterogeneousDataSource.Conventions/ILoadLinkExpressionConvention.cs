using System.Reflection;

namespace HeterogeneousDataSource.Conventions
{
    public interface ILoadLinkExpressionConvention
    {
        bool DoesApply(PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty);
    }
}