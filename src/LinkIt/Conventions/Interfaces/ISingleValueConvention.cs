﻿// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;

namespace LinkIt.Conventions.Interfaces
{
    /// <summary>
    /// Convention for loading a reference or linked source using an ID from the model.
    /// </summary>
    public interface ISingleValueConvention : ILoadLinkExpressionConvention
    {
        /// <summary>
        /// Apply the convention for a property of the model and a link.
        /// </summary>
        void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty,
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            PropertyInfo linkedSourceModelProperty,
            PropertyInfo linkTargetProperty
        );
    }
}