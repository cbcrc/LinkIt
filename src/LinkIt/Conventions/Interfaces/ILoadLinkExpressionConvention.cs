// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Reflection;

namespace LinkIt.Conventions.Interfaces
{
    public interface ILoadLinkExpressionConvention
    {
        string Name { get; }
        bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty);
    }
}