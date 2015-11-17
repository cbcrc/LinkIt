using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinkIt.Conventions.Interfaces;
using LinkIt.LinkedSources;
using LinkIt.LinkTargets;

namespace LinkIt.Conventions {
    public class FindAllConventionMatchesQuery{
        private readonly List<Type> _types;
        private readonly List<ILoadLinkExpressionConvention> _conventions;

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
                .Where(property=>property.Name != "Model")
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

            try{
                return match.Convention.DoesApply(match.LinkedSourceModelProperty, match.LinkTargetProperty);
            }
            catch (Exception ex){
                throw new Exception(
                    string.Format(
                        "The convention \"{0}\" failed for DoesApply. Link target id: {1}, linked source model property: {2}",
                        match.Convention.Name,
                        match.LinkTargetProperty.GetLinkTargetId(),
                        match.LinkedSourceModelProperty.Name
                    ),
                    ex
                ); 
            }
        }

        private Type GetPossibleConventionType(ConventionMatch match) {
            if (Nullable.GetUnderlyingType(match.LinkedSourceModelProperty.PropertyType) != null) {
                return typeof(IByNullableValueTypeIdConvention);
            }
            if (IsGenericList(match.LinkTargetProperty)) {
                return typeof(IMultiValueConvention);
            }
            return typeof(ISingleValueConvention);
        }

        private static bool IsGenericList(PropertyInfo linkTargetProperty) {
            if (!linkTargetProperty.PropertyType.IsGenericType) { return false; }

            return linkTargetProperty.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}