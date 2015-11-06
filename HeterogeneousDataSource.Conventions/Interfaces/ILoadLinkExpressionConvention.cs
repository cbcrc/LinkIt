using System.Reflection;

namespace HeterogeneousDataSource.Conventions.Interfaces
{
    public interface ILoadLinkExpressionConvention{
        string Name { get; }
        bool DoesApply(PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty);
    }
}