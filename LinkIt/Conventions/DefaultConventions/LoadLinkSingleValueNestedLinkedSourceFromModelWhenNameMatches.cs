#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.ConfigBuilders;
using LinkIt.Conventions.Interfaces;
using LinkIt.Shared;

namespace LinkIt.Conventions.DefaultConventions
{
    public class LoadLinkSingleValueNestedLinkedSourceFromModelWhenNameMatches : ISingleValueConvention
    {
        public string Name => "Load link single value nested linked source from model source when name matches";

        public bool DoesApply(PropertyInfo linkedSourceModelProperty, PropertyInfo linkTargetProperty)
        {
            if (linkTargetProperty.Name != linkedSourceModelProperty.Name) return false;
            if (!linkTargetProperty.PropertyType.DoesImplementILinkedSourceOnceAndOnlyOnce()) return false;
            if (linkTargetProperty.PropertyType.GetLinkedSourceModelType() != linkedSourceModelProperty.PropertyType) return false;

            return true;
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder,
            Func<TLinkedSource, TLinkedSourceModelProperty> getLinkedSourceModelProperty,
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            PropertyInfo linkedSourceModelProperty,
            PropertyInfo linkTargetProperty)
        {
            loadLinkProtocolForLinkedSourceBuilder.PolymorphicLoadLink<TLinkTargetProperty, TLinkedSourceModelProperty, bool>(
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