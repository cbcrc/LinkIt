using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Shared
{
    public static class LinkedSourceTypeExtensions
    {
        public static bool DoesImplementILinkedSourceOnceAndOnlyOnce(this Type type) {
            return GetILinkedSourceTypes(type).Count == 1;
        }

        public static List<Type> GetILinkedSourceTypes(this Type type) {
            var iLinkedSourceTypes = type.GetInterfaces()
                .Where(interfaceType =>
                    interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(ILinkedSource<>))
                .ToList();
            return iLinkedSourceTypes;
        }
    }
}