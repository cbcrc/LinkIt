#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Reflection;

namespace LinkIt.Shared
{
    public static class PropertyInfoExtensions
    {
        public static bool IsPublicReadWrite(this PropertyInfo property)
        {
            return property.CanRead &&
                   property.GetGetMethod(false) != null &&
                   property.CanWrite &&
                   property.GetSetMethod(false) != null;
        }

        public static string GetFullName(this PropertyInfo property)
        {
            return $"{property.DeclaringType}/{property.Name}";
        }
    }
}