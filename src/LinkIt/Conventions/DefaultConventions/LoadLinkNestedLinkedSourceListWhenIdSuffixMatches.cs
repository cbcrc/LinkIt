﻿// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.PublicApi;

namespace LinkIt.Conventions.DefaultConventions
{
    /// <summary>
    /// Load link the property of a linked source if it's linked sources and
    /// there is a property from the model with the same name suffixed by "Id[s]".
    /// </summary>
    public class LoadLinkNestedLinkedSourceListWhenIdSuffixMatches : INestedLinkedSourceListConvention
    {
        /// <inheritdoc />
        public string Name => "Load link nested linked sources when id suffix matches";

        /// <inheritdoc />
        public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
        {
            return linkTargetProperty.MatchLinkedSourceModelPropertyName(linkedSourceModelProperty, "Id", "s");
        }

        /// <inheritdoc />
        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Func<TLinkedSource, IEnumerable<TLinkedSourceModelProperty>> getLinkedSourceModelProperty,
            Expression<Func<TLinkedSource, List<TLinkTargetProperty>>> getLinkTargetProperty,
            PropertyInfo linkedSourceModelProperty,
            PropertyInfo linkTargetProperty)
            where TLinkedSource: ILinkedSource
            where TLinkTargetProperty: ILinkedSource
        {
            loadLinkProtocolForLinkedSourceBuilder.LoadLinkNestedLinkedSourcesByIds(
                getLinkedSourceModelProperty,
                getLinkTargetProperty
            );
        }
    }
}