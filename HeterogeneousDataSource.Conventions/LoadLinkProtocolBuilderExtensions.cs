using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions
{
    public static class LoadLinkProtocolBuilderExtensions
    {
        //stle: should not be public
        public static List<Type> GetLinkedSourceTypes(
            this LoadLinkProtocolBuilder loadLinkProtocolBuilder, 
            IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(LinkedSourceConfigs.DoesImplementILinkedSourceOnceAndOnlyOnce)
                .ToList();
        }

        public static List<PropertyInfo> GetLinkTargetProperties(Type linkedSourceType)
        {
            return linkedSourceType
                .GetProperties()
                .Where(PropertyInfoExtensions.IsPublicReadWrite)
                .ToList();
        }

        public static List<PropertyInfo> GetLinkedSourceModelProperties(Type linkedSourceType)
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
