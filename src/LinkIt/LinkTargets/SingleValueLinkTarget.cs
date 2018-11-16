// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.ReadableExpressions;
using LinkIt.Shared;

namespace LinkIt.LinkTargets
{
    internal class SingleValueLinkTarget<TLinkedSource, TTargetProperty> : ILinkTarget<TLinkedSource, TTargetProperty>
    {
        private readonly PropertyInfo _property;
        private readonly Expression<Func<TLinkedSource, TTargetProperty>> _expression;

        public SingleValueLinkTarget(PropertyInfo property, Expression<Func<TLinkedSource, TTargetProperty>> expression)
        {
            _property = property;
            _expression = expression;
            Id = property.GetFullName();
        }

        public string Id { get; }

        public string Expression => _expression.ToReadableString();

        public void SetLinkTargetValue(TLinkedSource linkedSource, TTargetProperty linkTargetValue, int linkTargetValueIndex)
        {
            _property.SetValue(linkedSource, linkTargetValue);
        }

        public TTargetProperty GetLinkTargetValue(TLinkedSource linkedSource, int linkTargetValueIndex)
        {
            return (TTargetProperty) _property.GetValue(linkedSource);
        }

        public void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValues)
        {
            //Do nothing for single value
        }

        public void FilterOutNullValues(TLinkedSource linkedSource)
        {
            //Do nothing for single value
        }

        public bool Equals(ILinkTarget other)
        {
            return other != null && Id.Equals(other.Id);
        }
    }
}
