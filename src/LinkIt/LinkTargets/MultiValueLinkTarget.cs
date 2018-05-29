// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.Shared;

namespace LinkIt.LinkTargets
{
    internal class MultiValueLinkTarget<TLinkedSource, TTargetProperty> : ILinkTarget<TLinkedSource, TTargetProperty>
    {
        private readonly Func<TLinkedSource, IList<TTargetProperty>> _get;
        private readonly PropertyInfo _property;

        public MultiValueLinkTarget(PropertyInfo property, Func<TLinkedSource, IList<TTargetProperty>> get)
        {
            _property = property;
            Id = property.GetFullName();
            _get = get;
        }

        //See ILinkTarget.SetLinkTargetValue
        public void SetLinkTargetValue(TLinkedSource linkedSource, TTargetProperty linkTargetValue, int linkTargetValueIndex)
        {
            _get(linkedSource)[linkTargetValueIndex] = linkTargetValue;
        }

        public void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValues)
        {
            if (_get(linkedSource) == null)
            {
                var polymorphicListToBeBuilt = new TTargetProperty[numOfLinkedTargetValues];
                SetTargetProperty(linkedSource, polymorphicListToBeBuilt);
            }
        }

        public void FilterOutNullValues(TLinkedSource linkedSource)
        {
            var values = _get(linkedSource);
            var valuesWithoutNull = values
                .Where(value => !value.EqualsDefaultValue())
                .ToArray();

            SetTargetProperty(linkedSource, valuesWithoutNull);
        }

        private void SetTargetProperty(TLinkedSource linkedSource, TTargetProperty[] values)
        {
            if (_property.PropertyType.IsAssignableFrom(typeof(TTargetProperty[])))
            {
                _property.SetMethod.Invoke(linkedSource, new object[] { values });
            }
            else if (_property.PropertyType.IsAssignableFrom(typeof(List<TTargetProperty>)))
            {
                _property.SetMethod.Invoke(linkedSource, new object[] { values.ToList() });
            }
        }

        public string Id { get; }

        public bool Equals(ILinkTarget other)
        {
            if (other == null)
            {
                return false;
            }

            return Id.Equals(other.Id);
        }
    }
}