#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

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
    public class LoadLinkMultiValueNestedLinkedSourceFromModelWhenNameMatches : IMultiValueConvention
    {
        public string Name => "Load link multi value nested linked source from model when name matches";

        public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
        {
            if (linkTargetProperty.Name != linkedSourceModelProperty.Name) return false;

            var sourceListItemType = linkedSourceModelProperty.PropertyType.GenericTypeArguments.Single();
            var linkTargetListItemType = linkTargetProperty.PropertyType.GenericTypeArguments.Single();

            if (!linkTargetListItemType.DoesImplementILinkedSourceOnceAndOnlyOnce()) return false;

            if (linkTargetListItemType.GetLinkedSourceModelType() != sourceListItemType) return false;

            return true;
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Func<TLinkedSource, List<TLinkedSourceModelProperty>> getLinkedSourceModelProperty,
            Expression<Func<TLinkedSource, List<TLinkTargetProperty>>> getLinkTargetProperty,
            PropertyInfo linkedSourceModelProperty,
            PropertyInfo linkTargetProperty)
        {
            loadLinkProtocolForLinkedSourceBuilder.PolymorphicLoadLinkForList(
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