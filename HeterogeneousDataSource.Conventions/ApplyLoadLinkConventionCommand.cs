using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions
{
    public class ApplyLoadLinkConventionCommand
    {
        private readonly List<Type> _linkedSourceTypes;
        private readonly LoadLinkProtocolBuilder _loadLinkProtocolBuilder;
        private readonly List<ILoadLinkExpressionConvention> _conventions;

        public ApplyLoadLinkConventionCommand(LoadLinkProtocolBuilder loadLinkProtocolBuilder, List<Type> types, List<ILoadLinkExpressionConvention> conventions)
        {
            _linkedSourceTypes = types
                .Where(LinkedSourceConfigs.DoesImplementILinkedSourceOnceAndOnlyOnce)
                .ToList();
            _loadLinkProtocolBuilder = loadLinkProtocolBuilder;
            _conventions = conventions;
        }

        public void Execute(){
            _linkedSourceTypes.ForEach(ApplyConvention);
        }

        private void ApplyConvention(Type linkedSourceType) {
            foreach (var linkTargetProperty in GetLinkTargetProperties(linkedSourceType)){
                foreach (var linkedSourceModelProperty in GetLinkedSourceModelProperties(linkedSourceType)) {
                    foreach (var convention in _conventions) {
                        if (convention.DoesApply(linkTargetProperty, linkedSourceModelProperty)){
                            convention.Apply(_loadLinkProtocolBuilder,linkedSourceType, linkTargetProperty, linkedSourceModelProperty);
                        }
                    }
                }
            }
        }

        private List<PropertyInfo> GetLinkTargetProperties(Type linkedSourceType)
        {
            return linkedSourceType
                .GetProperties()
                .Where(PropertyInfoExtensions.IsPublicReadWrite)
                .ToList();
        }

        private List<PropertyInfo> GetLinkedSourceModelProperties(Type linkedSourceType)
        {
            var linkedSourceModelType = linkedSourceType
                .GetProperty("Model")
                .PropertyType;

            return linkedSourceModelType
                .GetProperties()
                .ToList();
        }
    }
}