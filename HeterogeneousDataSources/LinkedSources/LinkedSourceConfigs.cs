using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LinkedSources
{
    public static class LinkedSourceConfigs
    {
        private static readonly Dictionary<Type, ILinkedSourceConfig> LinkedSourceConfigByType = new Dictionary<Type, ILinkedSourceConfig>();

        public static IGenericLinkedSourceConfig<TLinkedSource> GetConfigFor<TLinkedSource>()
        {
            return (IGenericLinkedSourceConfig<TLinkedSource>)GetConfigFor(typeof(TLinkedSource));
        }

        public static bool DoesImplementILinkedSourceOnceAndOnlyOnce(Type linkedSourceType) {
            return GetILinkedSourceTypes(linkedSourceType).Count == 1;
        }

        public static ILinkedSourceConfig GetConfigFor(Type linkedSourceType) {
            if (!LinkedSourceConfigByType.ContainsKey(linkedSourceType)) {
                LinkedSourceConfigByType.Add(linkedSourceType, CreateLinkedSourceConfig(linkedSourceType));
            }

            return LinkedSourceConfigByType[linkedSourceType];
        }

        private static ILinkedSourceConfig CreateLinkedSourceConfig(Type linkedSourceType)
        {
            Type[] typeArgs ={
                linkedSourceType,
                GetLinkedSourceModelType(linkedSourceType)
            };

            Type ctorGenericType = typeof(LinkedSourceConfig<,>);
            Type ctorSpecificType = ctorGenericType.MakeGenericType(typeArgs);

            var ctor = ctorSpecificType.GetConstructors().Single();
            var uncasted = ctor.Invoke(new object[0]);
            return (ILinkedSourceConfig)uncasted;
        }

        private static Type GetLinkedSourceModelType(Type linkedSourceType)
        {
            EnsureImplementsILinkedSourceOnceAndOnlyOnce(linkedSourceType);

            var iLinkedSourceTypes = GetILinkedSourceTypes(linkedSourceType);
            var iLinkedSourceType = iLinkedSourceTypes.Single();
            return iLinkedSourceType.GenericTypeArguments.Single();
        }

        private static void EnsureImplementsILinkedSourceOnceAndOnlyOnce(Type linkedSourceType)
        {
            if (!DoesImplementILinkedSourceOnceAndOnlyOnce(linkedSourceType)) {
                throw new ArgumentException(
                    string.Format(
                        "{0} must implement ILinkedSource<> once and only once.",
                        linkedSourceType
                    ),
                    "TLinkedSource"
                );
            }
        }

        private static List<Type> GetILinkedSourceTypes(Type linkedSourceType) {
            var iLinkedSourceTypes = linkedSourceType.GetInterfaces()
                .Where(interfaceType =>
                    interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(ILinkedSource<>))
                .ToList();
            return iLinkedSourceTypes;
        }
    }
}