// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Reflection;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.Shared;

namespace LinkIt.LinkTargets
{
    internal class SingleValueLinkTarget<TLinkedSource, TTargetProperty> : ILinkTarget<TLinkedSource, TTargetProperty>
    {
        private readonly PropertyInfo _property;

        public SingleValueLinkTarget(PropertyInfo property)
        {
            _property = property;
            Id = property.GetFullName();
        }

        public void SetLinkTargetValue(TLinkedSource linkedSource, TTargetProperty linkTargetValue, int linkTargetValueIndex)
        {
            _property.SetMethod.Invoke(linkedSource, new object[] { linkTargetValue });
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