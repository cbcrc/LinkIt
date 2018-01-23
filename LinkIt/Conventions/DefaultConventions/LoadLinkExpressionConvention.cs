#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Conventions.Interfaces;

namespace LinkIt.Conventions.DefaultConventions
{
    public static class LoadLinkExpressionConvention
    {
        public static List<ILoadLinkExpressionConvention> Default => new List<ILoadLinkExpressionConvention>
        {
            new LoadLinkByNullableValueTypeIdWhenIdSuffixMatches(),
            new LoadLinkMultiValueWhenIdSuffixMatches(),
            new LoadLinkSingleValueWhenIdSuffixMatches(),
            new LoadLinkMultiValueNestedLinkedSourceFromModelWhenNameMatches(),
            new LoadLinkSingleValueNestedLinkedSourceFromModelWhenNameMatches()
        };

        public static List<ILoadLinkExpressionConvention> DefaultAnd(params ILoadLinkExpressionConvention[] customConventions)
        {
            if (customConventions == null) throw new ArgumentNullException(nameof(customConventions));

            return Default
                .Concat(customConventions)
                .ToList();
        }
    }
}