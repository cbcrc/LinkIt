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
    /// Load link a linked source property of a linked source if there is a property from the model with the same name.
    /// </summary>
    public class LoadLinkSingleValueNestedLinkedSourceFromModelWhenNameMatches : ISingleValueConvention
    {
        /// <inheritdoc />
        public string Name => "Load link single value nested linked source from model source when name matches";

        /// <inheritdoc />
        public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
        {
            if (linkTargetProperty.Name != linkedSourceModelProperty.Name)
            {
                return false;
            }

            if (!linkTargetProperty.PropertyType.IsLinkedSource())
            {
                return false;
            }

            if (linkTargetProperty.PropertyType.GetLinkedSourceModelType() != linkedSourceModelProperty.PropertyType)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty,
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            PropertyInfo linkedSourceModelProperty,
            PropertyInfo linkTargetProperty)
        {
            loadLinkProtocolForLinkedSourceBuilder.LoadLinkPolymorphic<TLinkTargetProperty, TLinkedSourceModelProperty, bool>(
                getLinkedSourceModelProperty,
                getLinkTargetProperty,
                _ => true,
                includes => includes
                    .Include<TLinkTargetProperty>().AsNestedLinkedSourceFromModel(
                        true,
                        link => link
                    )
            );
        }
    }
}