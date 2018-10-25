// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Linq;
using LinkIt.Shared;

namespace LinkIt.Core
{
    /// <summary>
    /// Responsible for giving access to any linked source config.
    /// Responsible for creating linked source config.
    /// Responsible for inferring the linked source model type.
    /// </summary>
    internal static class LinkedSourceConfigs
    {
        private static readonly ConcurrentDictionary<Type, ILinkedSourceConfig> LinkedSourceConfigByType = new ConcurrentDictionary<Type, ILinkedSourceConfig>();

        public static IGenericLinkedSourceConfig<TLinkedSource> GetConfigFor<TLinkedSource>()
        {
            return (IGenericLinkedSourceConfig<TLinkedSource>) GetConfigFor(typeof(TLinkedSource));
        }

        public static ILinkedSourceConfig GetConfigFor(Type linkedSourceType)
        {
            return LinkedSourceConfigByType.GetOrAdd(linkedSourceType, CreateLinkedSourceConfig);
        }

        private static ILinkedSourceConfig CreateLinkedSourceConfig(Type linkedSourceType)
        {
            Type[] typeArgs =
            {
                linkedSourceType,
                linkedSourceType.GetLinkedSourceModelType()
            };

            var ctorGenericType = typeof(LinkedSourceConfig<,>);
            var ctorSpecificType = ctorGenericType.MakeGenericType(typeArgs);

            var ctor = ctorSpecificType.GetConstructors().Single();
            var uncasted = ctor.Invoke(new object[0]);
            return (ILinkedSourceConfig) uncasted;
        }
    }
}
