// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.PublicApi;
using LinkIt.Shared;

namespace LinkIt.Conventions.DefaultConventions
{
    /// <summary>
    /// Load link a linked source property of a linked source if there is a property from the model with the same name.
    /// </summary>
    public class LoadLinkNestedLinkedSourceListFromModelWhenNameMatches : INestedLinkedSourceListConvention
    {
        /// <inheritdoc />
        public string Name => "Load link nested linked source polymorphic list from model when name matches";

        /// <inheritdoc />
        public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
        {
            if (linkTargetProperty.Name != linkedSourceModelProperty.Name)
            {
                return false;
            }

            var sourceListItemType = linkedSourceModelProperty.PropertyType.GetEnumerableItemType();
            var linkTargetListItemType = linkTargetProperty.PropertyType.GetEnumerableItemType();
            if (linkTargetListItemType.GetLinkedSourceModelType() != sourceListItemType)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Func<TLinkedSource, IEnumerable<TLinkedSourceModelProperty>> getLinkedSourceModelProperty,
            Expression<Func<TLinkedSource, IList<TLinkTargetProperty>>> getLinkTargetProperty,
            PropertyInfo linkedSourceModelProperty,
            PropertyInfo linkTargetProperty)
            where TLinkedSource: ILinkedSource
            where TLinkTargetProperty: ILinkedSource
        {
            loadLinkProtocolForLinkedSourceBuilder.LoadLinkPolymorphicList(
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