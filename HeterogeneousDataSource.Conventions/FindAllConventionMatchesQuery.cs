using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HeterogeneousDataSource.Conventions.Interfaces;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions {
    public class FindAllConventionMatchesQuery{
        private List<Type> _types;
        private List<ILoadLinkExpressionConvention> _conventions;

        public FindAllConventionMatchesQuery(List<Type> types, List<ILoadLinkExpressionConvention> conventions)
        {
            _types = types;
            _conventions = conventions;
        }

        public List<ConventionMatch> Execute() {
            var possibleMatches =
                from linkedSourceType in GetLinkedSourceTypes()
                from linkTargetProperty in GetLinkTargetProperties(linkedSourceType)
                from linkedSourceModelProperty in GetLinkedSourceModelProperties(linkedSourceType)
                from convention in _conventions
                select new ConventionMatch(
                    convention,
                    linkedSourceType,
                    linkTargetProperty,
                    linkedSourceModelProperty
                );

            return possibleMatches
                .Where(DoesConventionApply)
                .ToList();
        }

        private List<Type> GetLinkedSourceTypes()
        {
            return _types
                .Where(LinkedSourceConfigs.DoesImplementILinkedSourceOnceAndOnlyOnce)
                .ToList();
        }

        private List<PropertyInfo> GetLinkTargetProperties(Type linkedSourceType) {
            return linkedSourceType
                .GetProperties()
                .Where(PropertyInfoExtensions.IsPublicReadWrite)
                .ToList();
        }

        private List<PropertyInfo> GetLinkedSourceModelProperties(Type linkedSourceType) {
            var linkedSourceModelType = linkedSourceType
                .GetProperty("Model")
                .PropertyType;

            return linkedSourceModelType
                .GetProperties()
                .ToList();
        }

        private bool DoesConventionApply(ConventionMatch match) {
            var possibleConventionType = GetPossibleConventionType(match);
            if (!possibleConventionType.IsInstanceOfType(match.Convention)) {
                return false;
            }

            return match.Convention.DoesApply(match.LinkTargetProperty, match.LinkedSourceModelProperty);
        }

        private Type GetPossibleConventionType(ConventionMatch match) {
            if (Nullable.GetUnderlyingType(match.LinkedSourceModelProperty.PropertyType) != null) {
                return typeof(IByNullableValueTypeIdConvention);
            }
            if (match.LinkTargetProperty.IsGenericList()) {
                return typeof(IMultiValueConvention);
            }
            return typeof(ISingleValueConvention);
        }
    }
}