using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources
{
    public static class LinkedSourceConfigs
    {
        private static readonly Dictionary<Type, object> LinkedSourceConfigByType = new Dictionary<Type, object>();

        public static ILinkedSourceConfig<TLinkedSource> GetConfigFor<TLinkedSource>()
        {
            var linkedSourceType = typeof(TLinkedSource);

            //Lazy init to minimize required configuration by the client.
            //stle: dangerous for multithreading
            if (!LinkedSourceConfigByType.ContainsKey(linkedSourceType)){
                LinkedSourceConfigByType.Add(linkedSourceType, CreateLinkedSourceConfig<TLinkedSource>());
            }

            return (ILinkedSourceConfig<TLinkedSource>)LinkedSourceConfigByType[linkedSourceType];
        }

        private static ILinkedSourceConfig<TLinkedSource> CreateLinkedSourceConfig<TLinkedSource>()
        {
            var linkedSourceType = typeof(TLinkedSource);

            Type[] typeArgs ={
                linkedSourceType,
                GetLinkedSourceModelType(linkedSourceType)
            };

            Type ctorGenericType = typeof(LinkedSourceConfig<,>);
            Type ctorSpecificType = ctorGenericType.MakeGenericType(typeArgs);

            var ctor = ctorSpecificType.GetConstructors().Single();
            var uncasted = ctor.Invoke(new object[0]);
            return (ILinkedSourceConfig<TLinkedSource>)uncasted;
        }

        private static Type GetLinkedSourceModelType(Type linkedSourceType)
        {
            var iLinkedSourceTypes = linkedSourceType.GetInterfaces()
                .Where(interfaceType =>
                    interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(ILinkedSource<>))
                .ToList();

            EnsureILinkedSourceIsImplementedOnceAndOnlyOnce(linkedSourceType, iLinkedSourceTypes);

            var iLinkedSourceType = iLinkedSourceTypes.Single();
            return iLinkedSourceType.GenericTypeArguments.Single();
        }

        private static void EnsureILinkedSourceIsImplementedOnceAndOnlyOnce(Type linkedSourceType, List<Type> iLinkedSourceTypes)
        {
            if (!iLinkedSourceTypes.Any())
            {
                throw new ArgumentException(
                    string.Format(
                        "{0} must implement ILinkedSource<>.",
                        linkedSourceType
                    ),
                    "TLinkedSource"
                );
            }

            if (iLinkedSourceTypes.Count > 1)
            {
                throw new ArgumentException(
                    string.Format(
                        "{0} must implement ILinkedSource<> only once.",
                        linkedSourceType
                    ),
                    "TLinkedSource"
                );
            }
        }
    }
}