using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.Shared;

namespace LinkIt.Conventions
{
    public class ApplyLoadLinkConventionCommand
    {
        private readonly LoadLinkProtocolBuilder _loadLinkProtocolBuilder;
        private readonly List<ConventionMatch> _matches;

        public ApplyLoadLinkConventionCommand(LoadLinkProtocolBuilder loadLinkProtocolBuilder, List<ConventionMatch> matches)
        {
            _loadLinkProtocolBuilder = loadLinkProtocolBuilder;
            _matches = matches;
        }

        public void Execute(){
            foreach (var match in _matches) {
                ApplyConvention(match);
            }
        }

        private void ApplyConvention(ConventionMatch match) {
            try { 
                if (match.Convention is ISingleValueConvention) { ApplySingleValueConvention(match); }
                if (match.Convention is IMultiValueConvention) { ApplyMultiValueConvention(match); }
                if (match.Convention is IByNullableValueTypeIdConvention) { ApplyNullableValueTypeIdConvention(match); }
            }
            catch (TargetInvocationException ex) {
                throw new Exception(
                    string.Format(
                        "The convention \"{0}\" failed for Apply. Link target id: {1}, linked source model property: {2}",
                        match.Convention.Name,
                        match.LinkTargetProperty.GetFullName(),
                        match.LinkedSourceModelProperty.Name
                    ),
                    ex.InnerException
                );
            }
        }

        #region ApplySingleValueConvention
        private void ApplySingleValueConvention(ConventionMatch match) {
            var method = GetType().GetMethod("ApplySingleValueConventionGeneric");
            var genericMethod = method.MakeGenericMethod(
                match.LinkedSourceType,
                match.LinkTargetProperty.PropertyType,
                match.LinkedSourceModelProperty.PropertyType
            );

            genericMethod.Invoke(this, new object[] { match });
        }

        public void ApplySingleValueConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(ConventionMatch match)
        {
            var getLinkTargetProperty = FuncGenerator.
                GenerateFromGetterAsExpression<TLinkedSource, TLinkTargetProperty>(
                    match.LinkTargetProperty.Name
                );
            var getLinkedSourceModelProperty = FuncGenerator
                .GenerateFromGetter<TLinkedSource, TLinkedSourceModelProperty>(
                    string.Format("Model.{0}", match.LinkedSourceModelProperty.Name)
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
        private void ApplyMultiValueConvention(ConventionMatch match) {
            var method = GetType().GetMethod("ApplyMultiValueConventionGeneric");

            var genericMethod = method.MakeGenericMethod(
                match.LinkedSourceType,
                match.LinkTargetProperty.PropertyType.GenericTypeArguments.Single(),
                match.LinkedSourceModelProperty.PropertyType.GenericTypeArguments.Single()
            );

            genericMethod.Invoke(this, new object[] { match });
        }

        public void ApplyMultiValueConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(ConventionMatch match)
         {
            var getLinkTargetProperty = FuncGenerator.
                GenerateFromGetterAsExpression<TLinkedSource, List<TLinkTargetProperty>>(
                    match.LinkTargetProperty.Name
                );
            var getLinkedSourceModelProperty = FuncGenerator
                .GenerateFromGetter<TLinkedSource, List<TLinkedSourceModelProperty>>(
                    string.Format("Model.{0}", match.LinkedSourceModelProperty.Name)
                );

            var casted = (IMultiValueConvention)match.Convention;
            casted.Apply(
                _loadLinkProtocolBuilder.For<TLinkedSource>(),
                getLinkedSourceModelProperty,
                getLinkTargetProperty, match.LinkedSourceModelProperty, match.LinkTargetProperty);
        } 
        #endregion

        #region ApplyNullableValueTypeIdConvention
        private void ApplyNullableValueTypeIdConvention(ConventionMatch match) {
            var method = GetType().GetMethod("ApplyNullableValueTypeIdConventionGeneric");
            var genericMethod = method.MakeGenericMethod(
                match.LinkedSourceType,
                match.LinkTargetProperty.PropertyType,
                Nullable.GetUnderlyingType(match.LinkedSourceModelProperty.PropertyType)
            );

            genericMethod.Invoke(this, new object[] { match });
        }

        public void ApplyNullableValueTypeIdConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(ConventionMatch match)
            where TLinkedSourceModelProperty : struct {
            var getLinkTargetProperty = FuncGenerator.
                GenerateFromGetterAsExpression<TLinkedSource, TLinkTargetProperty>(
                    match.LinkTargetProperty.Name
                );
            var getLinkedSourceModelProperty = FuncGenerator
                .GenerateFromGetter<TLinkedSource, TLinkedSourceModelProperty?>(
                    string.Format("Model.{0}", match.LinkedSourceModelProperty.Name)
                );

            var casted = (IByNullableValueTypeIdConvention)match.Convention;
            casted.Apply(
                _loadLinkProtocolBuilder.For<TLinkedSource>(),
                getLinkedSourceModelProperty,
                getLinkTargetProperty, match.LinkedSourceModelProperty, match.LinkTargetProperty);
        } 
        #endregion
    }
}