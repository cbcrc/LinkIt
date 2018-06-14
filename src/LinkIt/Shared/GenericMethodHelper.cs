using System;
using System.Reflection;

namespace LinkIt.Shared
{
    internal static class GenericMethodHelper
    {
        public static MethodInfo MakeGenericMethod(this Type type, string methodName, params Type[] typeArguments)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (methodName is null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return method.MakeGenericMethod(typeArguments);
        }
    }
}
