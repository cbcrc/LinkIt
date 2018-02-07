// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public class LoadLinkMultiValueNestedLinkedSourceFromModelWhenNameMatches : IMultiValueConvention
    {
        /// <inheritdoc />
        public string Name => "Load link multi value nested linked source from model when name matches";

        /// <inheritdoc />
        public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
        {
            if (linkTargetProperty.Name != linkedSourceModelProperty.Name) return false;

            var sourceListItemType = linkedSourceModelProperty.PropertyType.GenericTypeArguments.Single();
            var linkTargetListItemType = linkTargetProperty.PropertyType.GenericTypeArguments.Single();

            if (!linkTargetListItemType.IsLinkedSource()) return false;

            if (linkTargetListItemType.GetLinkedSourceModelType() != sourceListItemType) return false;

            return true;
        }

        /// <inheritdoc />
        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Func<TLinkedSource, List<TLinkedSourceModelProperty>> getLinkedSourceModelProperty,
            Expression<Func<TLinkedSource, List<TLinkTargetProperty>>> getLinkTargetProperty,
            PropertyInfo linkedSourceModelProperty,
            PropertyInfo linkTargetProperty)
        {
            loadLinkProtocolForLinkedSourceBuilder.LoadLinkPolymorphicList<TLinkTargetProperty, TLinkedSourceModelProperty, bool>(
                getLinkedSourceModelProperty,
                getLinkTargetProperty,
                link => true,
                includes => includes
                    .Include<TLinkTargetProperty>().AsNestedLinkedSourceFromModel(
                        true,
                        link => link
                    )
            );
        }
    }
}