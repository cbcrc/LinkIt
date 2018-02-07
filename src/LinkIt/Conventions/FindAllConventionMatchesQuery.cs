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

        private List<Type> GetLinkedSourceTypes()
        {
            return _types
                .Where(LinkedSourceTypeExtensions.IsLinkedSource)
                .ToList();
        }

        private List<PropertyInfo> GetLinkTargetProperties(Type linkedSourceType)
        {
            return linkedSourceType
                .GetProperties()
                .Where(property => property.Name != "Model")
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

        private bool DoesConventionApply(ConventionMatch match)
        {
            var possibleConventionType = GetPossibleConventionType(match);
            if (!possibleConventionType.IsInstanceOfType(match.Convention)) return false;

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

        private Type GetPossibleConventionType(ConventionMatch match)
        {
            if (Nullable.GetUnderlyingType(match.LinkedSourceModelProperty.PropertyType) != null) return typeof(IByNullableValueTypeIdConvention);
            if (IsGenericList(match.LinkTargetProperty)) return typeof(IMultiValueConvention);
            return typeof(ISingleValueConvention);
        }

        private static bool IsGenericList(PropertyInfo linkTargetProperty)
        {
            if (!linkTargetProperty.PropertyType.IsGenericType) return false;

            return linkTargetProperty.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}