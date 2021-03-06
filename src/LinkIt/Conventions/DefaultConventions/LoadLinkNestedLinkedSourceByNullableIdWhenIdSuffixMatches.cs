﻿// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.PublicApi;

namespace LinkIt.Conventions.DefaultConventions
{
    /// <summary>
    /// Load link the property of a linked source if there is a property from the model with the same name suffixed by "Id".
    /// </summary>
    public class LoadLinkNestedLinkedSourceByNullableIdWhenIdSuffixMatches : INestedLinkedSourceByNullableIdConvention
    {
        /// <inheritdoc />
        public string Name => "Load link linked source by nullable value type id when id suffix matches";

        /// <inheritdoc />
        public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
        {
            return linkTargetProperty.MatchLinkedSourceModelPropertyName(linkedSourceModelProperty, "Id");
        }

        /// <inheritdoc />
        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Func<TLinkedSource, TLinkedSourceModelProperty?> getLinkedSourceModelProperty,
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            PropertyInfo linkedSourceModelProperty,
            PropertyInfo linkTargetProperty
        )
            where TLinkedSource: ILinkedSource
            where TLinkedSourceModelProperty : struct
            where TLinkTargetProperty : ILinkedSource
        {
            loadLinkProtocolForLinkedSourceBuilder.LoadLinkNestedLinkedSourceById(
                getLinkedSourceModelProperty,
                getLinkTargetProperty
            );
        }
    }
}