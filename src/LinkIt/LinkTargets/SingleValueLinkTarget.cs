// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using LinkIt.LinkTargets.Interfaces;

namespace LinkIt.LinkTargets
{
    internal class SingleValueLinkTarget<TLinkedSource, TTargetProperty> : ILinkTarget<TLinkedSource, TTargetProperty>
    {
        private readonly Action<TLinkedSource, TTargetProperty> _set;

        public SingleValueLinkTarget(
            string id,
            Action<TLinkedSource, TTargetProperty> set)
        {
            _set = set;
            Id = id;
        }

        public void SetLinkTargetValue(TLinkedSource linkedSource, TTargetProperty linkTargetValue, int linkTargetValueIndex)
        {
            _set(linkedSource, linkTargetValue);
        }

        public void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValues)
        {
            //Do nothing for single value
        }

        public void FilterOutNullValues(TLinkedSource linkedSource)
        {
            //Do nothing for single value
        }

        public string Id { get; }

        public bool Equals(ILinkTarget other)
        {
            if (other == null) return false;

            return Id.Equals(other.Id);
        }
    }
}