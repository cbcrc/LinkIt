// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Reflection;
using LinkIt.Conventions.Interfaces;

namespace LinkIt.Conventions
{
    public class ConventionMatch
    {
        public ConventionMatch(ILoadLinkExpressionConvention convention, Type linkedSourceType, PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty)
        {
            Convention = convention;
            LinkedSourceType = linkedSourceType;
            LinkTargetProperty = linkTargetProperty;
            LinkedSourceModelProperty = linkedSourceModelProperty;
        }

        public ILoadLinkExpressionConvention Convention { get; }
        public Type LinkedSourceType { get; }
        public PropertyInfo LinkTargetProperty { get; }
        public PropertyInfo LinkedSourceModelProperty { get; }
    }
}