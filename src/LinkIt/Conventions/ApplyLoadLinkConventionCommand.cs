// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
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
                if (match.Convention is ISingleValueConvention)
                {
                    ApplySingleValueConvention(match);
                }

                if (match.Convention is IMultiValueConvention)
                {
                    ApplyMultiValueConvention(match);
                }

                if (match.Convention is IByNullableValueTypeIdConvention)
                {
                    ApplyNullableValueTypeIdConvention(match);
                }
            }
            catch (TargetInvocationException ex)
            {
                throw new Exception(
                    $"The convention \"{match.Convention.Name}\" failed for Apply. Link target id: {match.LinkTargetProperty.GetFullName()}, linked source model property: {match.LinkedSourceModelProperty.Name}",
                    ex.InnerException
                );
            }
        }

        #region ApplySingleValueConvention

        private void ApplySingleValueConvention(ConventionMatch match)
        {
            var method = GetType().GetMethod(nameof(ApplySingleValueConventionGeneric));
            var genericMethod = method.MakeGenericMethod(
                match.LinkedSourceType,
                match.LinkTargetProperty.PropertyType,
                match.LinkedSourceModelProperty.PropertyType
            );

            genericMethod.Invoke(this, new object[] { match });
        }

        public void ApplySingleValueConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(ConventionMatch match)
        {
            var getLinkTargetProperty = FuncGenerator.GenerateFromGetterAsExpression<TLinkedSource, TLinkTargetProperty>(
                match.LinkTargetProperty.Name
            );
            var getLinkedSourceModelProperty = FuncGenerator
                .GenerateFromGetter<TLinkedSource, TLinkedSourceModelProperty>(
                    $"Model.{match.LinkedSourceModelProperty.Name}"
                );

            var casted = (ISingleValueConvention) match.Convention;
            casted.Apply(
                _loadLinkProtocolBuilder.For<TLinkedSource>(),
                getLinkedSourceModelProperty,
                getLinkTargetProperty,
                match.LinkedSourceModelProperty, match.LinkTargetProperty);
        }

        #endregion

        #region ApplyMultiValueConvention

        private void ApplyMultiValueConvention(ConventionMatch match)
        {
            var method = GetType().GetMethod(nameof(ApplyMultiValueConventionGeneric));

            var genericMethod = method.MakeGenericMethod(
                match.LinkedSourceType,
                match.LinkTargetProperty.PropertyType.GenericTypeArguments.Single(),
                match.LinkedSourceModelProperty.PropertyType.GenericTypeArguments.Single()
            );

            genericMethod.Invoke(this, new object[] { match });
        }

        public void ApplyMultiValueConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(ConventionMatch match)
        {
            var getLinkTargetProperty = FuncGenerator.GenerateFromGetterAsExpression<TLinkedSource, IList<TLinkTargetProperty>>(
                match.LinkTargetProperty.Name
            );
            var getLinkedSourceModelProperty = FuncGenerator
                .GenerateFromGetter<TLinkedSource, IList<TLinkedSourceModelProperty>>(
                    $"Model.{match.LinkedSourceModelProperty.Name}"
                );

            var casted = (IMultiValueConvention) match.Convention;
            casted.Apply(
                _loadLinkProtocolBuilder.For<TLinkedSource>(),
                getLinkedSourceModelProperty,
                getLinkTargetProperty, match.LinkedSourceModelProperty, match.LinkTargetProperty);
        }

        #endregion

        #region ApplyNullableValueTypeIdConvention

        private void ApplyNullableValueTypeIdConvention(ConventionMatch match)
        {
            var method = GetType().GetMethod(nameof(ApplyNullableValueTypeIdConventionGeneric));
            var genericMethod = method.MakeGenericMethod(
                match.LinkedSourceType,
                match.LinkTargetProperty.PropertyType,
                Nullable.GetUnderlyingType(match.LinkedSourceModelProperty.PropertyType)
            );

            genericMethod.Invoke(this, new object[] { match });
        }

        public void ApplyNullableValueTypeIdConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(ConventionMatch match)
            where TLinkedSourceModelProperty : struct
        {
            var getLinkTargetProperty = FuncGenerator.GenerateFromGetterAsExpression<TLinkedSource, TLinkTargetProperty>(
                match.LinkTargetProperty.Name
            );
            var getLinkedSourceModelProperty = FuncGenerator
                .GenerateFromGetter<TLinkedSource, TLinkedSourceModelProperty?>(
                    $"Model.{match.LinkedSourceModelProperty.Name}"
                );

            var casted = (IByNullableValueTypeIdConvention) match.Convention;
            casted.Apply(
                _loadLinkProtocolBuilder.For<TLinkedSource>(),
                getLinkedSourceModelProperty,
                getLinkTargetProperty, match.LinkedSourceModelProperty, match.LinkTargetProperty);
        }

        #endregion
    }
}