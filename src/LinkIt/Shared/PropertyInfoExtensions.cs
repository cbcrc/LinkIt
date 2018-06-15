// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Reflection;
using LinkIt.ReadableExpressions.Extensions;

namespace LinkIt.Shared
{
    internal static class PropertyInfoExtensions
    {
        public static bool IsPublicReadWrite(this PropertyInfo property)
        {
            return property.CanRead
                   && property.GetGetMethod(false) != null
                   && property.CanWrite
                   && property.GetSetMethod(false) != null;
        }

        public static string GetFullName(this PropertyInfo property)
        {
            return $"{property.DeclaringType.GetFriendlyName()}.{property.Name}";
        }
    }
}