// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinkIt.Conventions.Interfaces;
using LinkIt.Shared;

namespace LinkIt.Conventions
{
    internal class FindAllConventionMatchesQuery
    {
        private readonly IList<ILoadLinkExpressionConvention> _conventions;
        private readonly IList<Type> _types;

        public FindAllConventionMatchesQuery(IList<Type> types, IList<ILoadLinkExpressionConvention> conventions)
        {
            _types = types;
            _conventions = conventions;
        }

        public List<ConventionMatch> Execute()
        {
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

        private IEnumerable<Type> GetLinkedSourceTypes()
        {
            return _types
                .Where(TypeExtensions.IsLinkedSource);
        }

        private static IEnumerable<PropertyInfo> GetLinkTargetProperties(Type linkedSourceType)
        {
            return linkedSourceType
                .GetProperties()
                .Where(property => property.Name != "Model")
                .Where(PropertyInfoExtensions.IsPublicReadWrite);
        }

        private static IEnumerable<PropertyInfo> GetLinkedSourceModelProperties(Type linkedSourceType)
        {
            var linkedSourceModelType = linkedSourceType
                .GetProperty("Model")
                .PropertyType;

            return linkedSourceModelType
                .GetProperties();
        }

        private static bool DoesConventionApply(ConventionMatch match)
        {
            var possibleConventionType = GetPossibleConventionType(match);
            if (!possibleConventionType.IsInstanceOfType(match.Convention))
            {
                return false;
            }

            try
            {
                return match.Convention.DoesApply(match.LinkedSourceModelProperty, match.LinkTargetProperty);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"The convention \"{match.Convention.Name}\" failed for DoesApply. Link target id: {match.LinkTargetProperty.GetFullName()}, linked source model property: {match.LinkedSourceModelProperty.Name}",
                    ex
                );
            }
        }

        private static Type GetPossibleConventionType(ConventionMatch match)
        {
            if (Nullable.GetUnderlyingType(match.LinkedSourceModelProperty.PropertyType) != null)
            {
                if (match.LinkTargetProperty.PropertyType.IsLinkedSource())
                {
                    return typeof(INestedLinkedSourceByNullableIdConvention);
                }

                return typeof(IReferenceByNullableIdConvention);
            }

            if (IsGenericList(match.LinkTargetProperty))
            {
                var linkTargetListItemType = match.LinkTargetProperty.PropertyType.GetEnumerableItemType();
                if (linkTargetListItemType.IsLinkedSource())
                {
                    return typeof(INestedLinkedSourceListConvention);
                }

                return typeof(IReferenceListConvention);
            }

            if (match.LinkTargetProperty.PropertyType.IsLinkedSource())
            {
                return typeof(INestedLinkedSourceConvention);
            }

            return typeof(IReferenceConvention);
        }

        private static bool IsGenericList(PropertyInfo linkTargetProperty)
        {
            return linkTargetProperty.PropertyType.IsGenericType
                && linkTargetProperty.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}