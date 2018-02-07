// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Shared
{
    internal static class LinkedSourceTypeExtensions
    {
        public static bool IsLinkedSource(this Type type)
        {
            return GetLinkedSourceInterface(type) != null;
        }

        private static Type GetLinkedSourceInterface(this Type type)
        {
            return type.GetInterfaces()
                .Where(i => i.IsGenericType)
                .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(ILinkedSource<>));
        }

        public static Type GetLinkedSourceModelType(this Type type)
        {
            var iLinkedSourceType = type.GetLinkedSourceInterface();
            if (iLinkedSourceType == null)
            {
                throw new ArgumentException($"{type.Name} must implement ILinkedSource<>.", nameof(type));
            }

            return iLinkedSourceType.GenericTypeArguments.Single();
        }
    }
}