#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Reflection;

namespace LinkIt.Conventions.Interfaces
{
    public interface ILoadLinkExpressionConvention{
        string Name { get; }
        bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty);
    }
}