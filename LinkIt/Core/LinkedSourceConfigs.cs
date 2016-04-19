#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

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
                linkedSourceType.GetLinkedSourceModelType()
            };

            Type ctorGenericType = typeof(LinkedSourceConfig<,>);
            Type ctorSpecificType = ctorGenericType.MakeGenericType(typeArgs);

            var ctor = ctorSpecificType.GetConstructors().Single();
            var uncasted = ctor.Invoke(new object[0]);
            return (ILinkedSourceConfig)uncasted;
        }
    }
}