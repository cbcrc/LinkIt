using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Core.Interfaces;
using LinkIt.Shared;

namespace LinkIt.Core
{
    //Responsible for giving access to any linked source config
    //Responsible for creating linked source config
    //Responsible for inferring the linked source model type
    public static class LinkedSourceConfigs
    {
        private static readonly Dictionary<Type, ILinkedSourceConfig> LinkedSourceConfigByType = new Dictionary<Type, ILinkedSourceConfig>();

        public static IGenericLinkedSourceConfig<TLinkedSource> GetConfigFor<TLinkedSource>()
        {
            return (IGenericLinkedSourceConfig<TLinkedSource>)GetConfigFor(typeof(TLinkedSource));
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

            var iLinkedSourceTypes = linkedSourceType.GetILinkedSourceTypes();
            var iLinkedSourceType = iLinkedSourceTypes.Single();
            return iLinkedSourceType.GenericTypeArguments.Single();
        }

        private static void EnsureImplementsILinkedSourceOnceAndOnlyOnce(Type linkedSourceType)
        {
            if (!linkedSourceType.DoesImplementILinkedSourceOnceAndOnlyOnce()){
                throw new ArgumentException(
                    string.Format(
                        "{0} must implement ILinkedSource<> once and only once.",
                        linkedSourceType
                    ),
                    "TLinkedSource"
                );
            }
        }
    }
}