// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.Shared;

namespace LinkIt.Conventions.DefaultConventions
{
    /// <summary>
    /// Load link the property of a linked source if there is a property from the model with the same name suffixed by "Id".
    /// </summary>
    public class LoadLinkByNullableValueTypeIdWhenIdSuffixMatches : IByNullableValueTypeIdConvention
    {
        /// <inheritdoc />
        public string Name => "Load link by nullable value type id when id suffix matches";

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
            where TLinkedSourceModelProperty : struct
        {
            if (typeof(TLinkTargetProperty).IsLinkedSource())
                loadLinkProtocolForLinkedSourceBuilder.LoadLinkNestedLinkedSourceById(
                    getLinkedSourceModelProperty,
                    getLinkTargetProperty
                );
            else
                loadLinkProtocolForLinkedSourceBuilder.LoadLinkReferenceById(
                    getLinkedSourceModelProperty,
                    getLinkTargetProperty
                );
        }
    }
}