// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Reflection;

namespace LinkIt.Conventions.Interfaces
{
    /// <summary>
    /// LoadLink convention
    /// </summary>
    public interface ILoadLinkExpressionConvention
    {
        /// <summary>
        /// Name/description of the convention
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Checks whether a convention can apply for a property of the model and a link target.
        /// </summary>
        bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty);
    }
}