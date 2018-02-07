// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.LinkTargets.Interfaces;

namespace LinkIt.LinkTargets
{
    public class MultiValueLinkTarget<TLinkedSource, TTargetProperty> : ILinkTarget<TLinkedSource, TTargetProperty>
    {
        private readonly Func<TLinkedSource, List<TTargetProperty>> _get;
        private readonly Action<TLinkedSource, List<TTargetProperty>> _set;

        public MultiValueLinkTarget(
            string id,
            Func<TLinkedSource, List<TTargetProperty>> get,
            Action<TLinkedSource, List<TTargetProperty>> set)
        {
            Id = id;
            _get = get;
            _set = set;
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
                var polymorphicListToBeBuilt = new TTargetProperty[numOfLinkedTargetValues].ToList();
                _set(linkedSource, polymorphicListToBeBuilt);
            }
        }

        public void FilterOutNullValues(TLinkedSource linkedSource)
        {
            var values = _get(linkedSource);
            var valuesWithoutNull = values
                .Where(value => value != null)
                .ToList();

            _set(linkedSource, valuesWithoutNull);
        }

        public string Id { get; }

        public bool Equals(ILinkTarget other)
        {
            if (other == null) return false;

            return Id.Equals(other.Id);
        }
    }
}