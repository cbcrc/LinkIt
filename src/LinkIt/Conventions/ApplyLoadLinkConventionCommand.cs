// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.PublicApi;
using LinkIt.Shared;

namespace LinkIt.Conventions
{
    internal class ApplyLoadLinkConventionCommand
    {
        private readonly LoadLinkProtocolBuilder _loadLinkProtocolBuilder;
        private readonly List<ConventionMatch> _matches;

        public ApplyLoadLinkConventionCommand(LoadLinkProtocolBuilder loadLinkProtocolBuilder, List<ConventionMatch> matches)
        {
            _loadLinkProtocolBuilder = loadLinkProtocolBuilder;
            _matches = matches;
        }

        public void Execute()
        {
            foreach (var match in _matches)
            {
                ApplyConvention(match);
            }
        }

        private void ApplyConvention(ConventionMatch match)
        {
            try
            {
                if (match.Convention is IReferenceConvention || match.Convention is INestedLinkedSourceConvention)
                {
                    ApplySingleValueConvention(match);
                }

                if (match.Convention is IReferenceListConvention || match.Convention is INestedLinkedSourceListConvention)
                {
                    ApplyListConvention(match);
                }

                if (match.Convention is IReferenceByNullableIdConvention || match.Convention is INestedLinkedSourceByNullableIdConvention)
                {
                    ApplyNullableIdConvention(match);
                }
            }
            catch (TargetInvocationException ex)
            {
                throw new LinkItException(
                    $"The convention \"{match.Convention.Name}\" failed for Apply. Link target id: {match.LinkTargetProperty.GetFullName()}, linked source model property: {match.LinkedSourceModelProperty.GetFullName()}",
                    ex.GetBaseException()
                );
            }
        }

        #region ApplySingleValueConvention

        private void ApplySingleValueConvention(ConventionMatch match)
        {
            var genericParameters = new []
            {
                match.LinkedSourceType,
                match.LinkTargetProperty.PropertyType,
                match.LinkedSourceModelProperty.PropertyType
            };

            var conventionApplyMethod = match.Convention.GetType().MakeGenericMethod(
                nameof(IReferenceConvention.Apply), genericParameters
            );

            var callConventionGenericMethod = GetType().MakeGenericMethod(
                nameof(ApplySingleValueConventionGeneric), genericParameters
            );

            callConventionGenericMethod.Invoke(this, new object[] { match, conventionApplyMethod });
        }

        public void ApplySingleValueConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(ConventionMatch match, MethodInfo applyMethod)
            where TLinkedSource: ILinkedSource
        {
            var getLinkTargetProperty = FuncGenerator.GenerateFromGetterAsExpression<TLinkedSource, TLinkTargetProperty>(
                match.LinkTargetProperty.Name
            );
            var getLinkedSourceModelProperty = FuncGenerator
                .GenerateFromGetter<TLinkedSource, TLinkedSourceModelProperty>(
                    $"Model.{match.LinkedSourceModelProperty.Name}"
                );

            var parameters = new object[]
            {
                _loadLinkProtocolBuilder.For<TLinkedSource>(),
                getLinkedSourceModelProperty,
                getLinkTargetProperty,
                match.LinkedSourceModelProperty,
                match.LinkTargetProperty
            };

            applyMethod.Invoke(match.Convention, parameters);
        }

        #endregion

        #region ApplyMultiValueConvention

        private void ApplyListConvention(ConventionMatch match)
        {
            var genericParameters = new []
            {
                match.LinkedSourceType,
                match.LinkTargetProperty.PropertyType.GetEnumerableItemType(),
                match.LinkedSourceModelProperty.PropertyType.GetEnumerableItemType()
            };

            var conventionApplyMethod = match.Convention.GetType().MakeGenericMethod(
                nameof(IReferenceListConvention.Apply), genericParameters
            );

            var callConventionGenericMethod = GetType().MakeGenericMethod(
                nameof(ApplyListConventionGeneric), genericParameters
            );

            callConventionGenericMethod.Invoke(this, new object[] { match, conventionApplyMethod });
        }

        public void ApplyListConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(ConventionMatch match, MethodInfo applyMethod)
            where TLinkedSource: ILinkedSource
        {
            var getLinkTargetProperty = FuncGenerator.GenerateFromGetterAsExpression<TLinkedSource, List<TLinkTargetProperty>>(
                match.LinkTargetProperty.Name
            );
            var getLinkedSourceModelProperty = FuncGenerator
                .GenerateFromGetter<TLinkedSource, IList<TLinkedSourceModelProperty>>(
                    $"Model.{match.LinkedSourceModelProperty.Name}"
                );

            var parameters = new object[]
            {
                _loadLinkProtocolBuilder.For<TLinkedSource>(),
                getLinkedSourceModelProperty,
                getLinkTargetProperty,
                match.LinkedSourceModelProperty,
                match.LinkTargetProperty
            };

            applyMethod.Invoke(match.Convention, parameters);
        }

        #endregion

        #region ApplyNullableValueTypeIdConvention

        private void ApplyNullableIdConvention(ConventionMatch match)
        {
            var genericParameters = new []
            {
                match.LinkedSourceType,
                match.LinkTargetProperty.PropertyType,
                Nullable.GetUnderlyingType(match.LinkedSourceModelProperty.PropertyType)
            };

            var conventionApplyMethod = match.Convention.GetType().MakeGenericMethod(
                nameof(IReferenceByNullableIdConvention.Apply), genericParameters
            );

            var callConventionGenericMethod = GetType().MakeGenericMethod(
                nameof(ApplyNullableIdConventionGeneric), genericParameters
            );

            callConventionGenericMethod.Invoke(this, new object[] { match, conventionApplyMethod });
        }

        public void ApplyNullableIdConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(ConventionMatch match, MethodInfo applyMethod)
            where TLinkedSource: ILinkedSource
            where TLinkedSourceModelProperty : struct
        {
            var parameters = new object[]
            {
                _loadLinkProtocolBuilder.For<TLinkedSource>(),
                FuncGenerator.GenerateFromGetter<TLinkedSource, TLinkedSourceModelProperty?>(
                    $"Model.{match.LinkedSourceModelProperty.Name}"
                ),
                FuncGenerator.GenerateFromGetterAsExpression<TLinkedSource, TLinkTargetProperty>(match.LinkTargetProperty.Name),
                match.LinkedSourceModelProperty,
                match.LinkTargetProperty
            };

            applyMethod.Invoke(match.Convention, parameters);
        }

        #endregion
    }
}