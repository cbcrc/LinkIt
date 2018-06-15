// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions.Translators
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal interface IMethodInfo
    {
        string Name { get; }

        bool IsGenericMethod { get; }

        bool IsExtensionMethod { get; }

        MethodInfo GetGenericMethodDefinition();

        IEnumerable<Type> GetGenericArguments();

        IEnumerable<ParameterInfo> GetParameters();

        Type GetGenericArgumentFor(Type parameterType);
    }
}