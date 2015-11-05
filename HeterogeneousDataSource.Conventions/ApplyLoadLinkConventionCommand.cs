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
                        if (convention.DoesApply(linkTargetProperty, linkedSourceModelProperty)){
                            ApplyConvention(convention,_loadLinkProtocolBuilder,linkedSourceType, linkTargetProperty, linkedSourceModelProperty);
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

        private void ApplyConvention(ILoadLinkExpressionConvention convention, LoadLinkProtocolBuilder loadLinkProtocolBuilder, Type linkedSourceType, PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty) 
        {
            var method = GetType().GetMethod("ApplyConventionGeneric");
            var genericMethod = method.MakeGenericMethod(
                linkedSourceType,
                linkTargetProperty.PropertyType,
                linkedSourceModelProperty.PropertyType
            );

            genericMethod.Invoke(null, new object[]{
                convention,
                loadLinkProtocolBuilder,
                linkTargetProperty,
                linkedSourceModelProperty
            });
        }

        public static void ApplyConventionGeneric<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            ILoadLinkExpressionConvention convention,
            LoadLinkProtocolBuilder loadLinkProtocolBuilder,
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty)
        {
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
                getLinkedSourceModelProperty,
                linkTargetProperty,
                linkedSourceModelProperty
            );
        }


        //public void Apply(
        //    LoadLinkProtocolBuilder loadLinkProtocolBuilder,
        //    Type linkedSourceType,
        //    PropertyInfo linkTargetProperty,
        //    PropertyInfo linkedSourceModelProperty) 
        //{
        //}

        //private static Func<TLinkedSource, TProperty> GenerateGetFunc<TLinkedSource, TProperty>(
        //    string sourcePropertiesPrefix,
        //    IEnumerable<PropertyInfo> nestedProperties,
        //    IMappingExpression<TLinkedSource, TDestination> expression) 
        //{
        //    foreach (var property in nestedProperties) {
        //        var sourcePropertyInDotNotation = string.Format("{0}.{1}", sourcePropertiesPrefix, property.Name);
        //        var method = ThisType.GetMethod("MapProperty");
        //        var genericMethod = method.MakeGenericMethod(property.PropertyType);

        //        genericMethod.Invoke(null, new object[]
        //        {
        //            sourcePropertyInDotNotation,
        //            property.Name,
        //            expression
        //        });
        //    }
        //}
    }
}