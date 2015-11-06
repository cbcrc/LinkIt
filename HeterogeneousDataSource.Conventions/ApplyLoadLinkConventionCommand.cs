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
            _linkedSourceTypes.ForEach(ApplyConventions);
        }

        private void ApplyConventions(Type linkedSourceType) {
            foreach (var linkTargetProperty in GetLinkTargetProperties(linkedSourceType)){
                foreach (var linkedSourceModelProperty in GetLinkedSourceModelProperties(linkedSourceType)) {
                    foreach (var convention in _conventions) {
                        ApplyConvention(
                            linkedSourceType, 
                            linkTargetProperty, 
                            linkedSourceModelProperty, 
                            convention
                        );
                    }
                }
            }
        }

        private void ApplyConvention(Type linkedSourceType, PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty, ILoadLinkExpressionConvention convention)
        {
            var possibleConventionType = GetPossibleConventionType(linkTargetProperty, linkedSourceModelProperty);
            if (!possibleConventionType.IsInstanceOfType(convention)){
                return;
            }

            if (!convention.DoesApply(linkTargetProperty, linkedSourceModelProperty)){
                return;
            }

            if (convention is ISingleValueConvention){
                //stle: have a type for params?
                ApplySingleValueConvention(
                    (ISingleValueConvention)convention, 
                    _loadLinkProtocolBuilder, 
                    linkedSourceType, 
                    linkTargetProperty, 
                    linkedSourceModelProperty
                );
            }
            if (convention is IMultiValueConvention) {
                //stle: have a type for params?
                ApplyMultiValueConvention(
                    (IMultiValueConvention)convention,
                    _loadLinkProtocolBuilder,
                    linkedSourceType,
                    linkTargetProperty,
                    linkedSourceModelProperty
                );
            }
        }

        public static Type GetPossibleConventionType(PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty) {
            if (Nullable.GetUnderlyingType(linkedSourceModelProperty.PropertyType) != null)
            {
                throw new NotImplementedException("STLE: todo");
            }

            if (linkTargetProperty.PropertyType.IsGenericType &&
                linkTargetProperty.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) 
            {
                return typeof(IMultiValueConvention);
            }

            return typeof(ISingleValueConvention);
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

        #region ApplySingleValueConvention
        private void ApplySingleValueConvention(ISingleValueConvention singleValueConvention, LoadLinkProtocolBuilder loadLinkProtocolBuilder, Type linkedSourceType, PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty) {
            var method = GetType().GetMethod("ApplySingleValueConventionGeneric");
            var genericMethod = method.MakeGenericMethod(
                linkedSourceType,
                linkTargetProperty.PropertyType,
                linkedSourceModelProperty.PropertyType
            );

            genericMethod.Invoke(null, new object[]{
                singleValueConvention,
                loadLinkProtocolBuilder,
                linkTargetProperty,
                linkedSourceModelProperty
            });
        }

        public static void ApplySingleValueConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            ISingleValueConvention convention,
            LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty) {
            var getLinkTargetProperty = FuncGenerator.
                GenerateFromGetterAsExpression<TLinkedSource, TLinkTargetProperty>(
                    linkTargetProperty.Name
                );
            var getLinkedSourceModelProperty = FuncGenerator
                .GenerateFromGetter<TLinkedSource, TLinkedSourceModelProperty>(
                    string.Format("Model.{0}", linkedSourceModelProperty.Name)
                );

            convention.Apply(
                loadLinkProtocolBuilder.For<TLinkedSource>(),
                getLinkTargetProperty,
                getLinkedSourceModelProperty
            );
        } 
        #endregion

        #region ApplyMultiValueConvention
        private void ApplyMultiValueConvention(IMultiValueConvention multiValueConvention, LoadLinkProtocolBuilder loadLinkProtocolBuilder, Type linkedSourceType, PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty) {
            var method = GetType().GetMethod("ApplyMultiValueConventionGeneric");

            var genericMethod = method.MakeGenericMethod(
                linkedSourceType,
                linkTargetProperty.PropertyType.GenericTypeArguments.Single(),
                linkedSourceModelProperty.PropertyType.GenericTypeArguments.Single()
            );

            genericMethod.Invoke(null, new object[]{
                multiValueConvention,
                loadLinkProtocolBuilder,
                linkTargetProperty,
                linkedSourceModelProperty
            });
        }

        public static void ApplyMultiValueConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            IMultiValueConvention convention,
            LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty) {
            var getLinkTargetProperty = FuncGenerator.
                GenerateFromGetterAsExpression<TLinkedSource, List<TLinkTargetProperty>>(
                    linkTargetProperty.Name
                );
            var getLinkedSourceModelProperty = FuncGenerator
                .GenerateFromGetter<TLinkedSource, List<TLinkedSourceModelProperty>>(
                    string.Format("Model.{0}", linkedSourceModelProperty.Name)
                );

            convention.Apply(
                loadLinkProtocolBuilder.For<TLinkedSource>(),
                getLinkTargetProperty,
                getLinkedSourceModelProperty
            );
        } 
        #endregion

        
    }
}