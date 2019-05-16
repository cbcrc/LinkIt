// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.ReadableExpressions;
using LinkIt.Shared;

namespace LinkIt.LinkTargets
{
    internal class MultiValueLinkTarget<TLinkedSource, TTargetProperty> : ILinkTarget<TLinkedSource, TTargetProperty>
    {
        private readonly Func<TLinkedSource, List<TTargetProperty>> _get;
        private readonly PropertyInfo _property;
        private readonly Expression<Func<TLinkedSource, List<TTargetProperty>>> _expression;

        public MultiValueLinkTarget(PropertyInfo property, Expression<Func<TLinkedSource, List<TTargetProperty>>> expression)
        {
            _property = property;
            _expression = expression;
            _get = expression.Compile();
            Id = property.GetFullName();
        }

        public string Id { get; }

        public string Expression => _expression.ToReadableString();

        //See ILinkTarget.SetLinkTargetValue
        public void SetLinkTargetValue(TLinkedSource linkedSource, TTargetProperty linkTargetValue, int linkTargetValueIndex)
        {
            var list = _get(linkedSource);
            list[linkTargetValueIndex] = linkTargetValue;
        }

        public TTargetProperty GetLinkTargetValue(TLinkedSource linkedSource, int linkTargetValueIndex)
        {
            var list = _get(linkedSource);
            if (list is null || linkTargetValueIndex >= list.Count)
            {
                return default;
            }

            return list[linkTargetValueIndex];
        }

        public void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValues)
        {
            if (_get(linkedSource) is null)
            {
                var polymorphicListToBeBuilt = new TTargetProperty[numOfLinkedTargetValues];
                SetTargetProperty(linkedSource, polymorphicListToBeBuilt.ToList());
            }
        }

        public void FilterOutNullValues(TLinkedSource linkedSource)
        {
            var values = _get(linkedSource);
            var valuesWithoutNull = values
                .WhereNotNull()
                .ToList();

            SetTargetProperty(linkedSource, valuesWithoutNull);
        }

        private void SetTargetProperty(TLinkedSource linkedSource, List<TTargetProperty> values)
        {
            _property.SetValue(linkedSource, values);
        }

        public bool Equals(ILinkTarget other)
        {
            return other != null && Id.Equals(other.Id);
        }
    }
}
