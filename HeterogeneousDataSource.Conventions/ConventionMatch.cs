using System;
using System.Reflection;

namespace HeterogeneousDataSource.Conventions
{
    public class ConventionMatch {
        public ConventionMatch(ILoadLinkExpressionConvention convention, Type linkedSourceType, PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty) {
            Convention = convention;
            LinkedSourceType = linkedSourceType;
            LinkTargetProperty = linkTargetProperty;
            LinkedSourceModelProperty = linkedSourceModelProperty;
        }

        public ILoadLinkExpressionConvention Convention { get; private set; }
        public Type LinkedSourceType { get; private set; }
        public PropertyInfo LinkTargetProperty { get; private set; }
        public PropertyInfo LinkedSourceModelProperty { get; private set; }
    }
}