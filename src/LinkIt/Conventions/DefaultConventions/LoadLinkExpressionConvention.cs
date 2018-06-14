// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Conventions.Interfaces;

namespace LinkIt.Conventions.DefaultConventions
{
    /// <summary>
    /// Helper class to get the built-in conventions of LinkIt.
    /// </summary>
    public static class LoadLinkExpressionConvention
    {
        /// <summary>
        /// List of built-in conventions.
        /// </summary>
        public static List<ILoadLinkExpressionConvention> Default => new List<ILoadLinkExpressionConvention>
        {
            new LoadLinkReferenceByNullableIdWhenIdSuffixMatches(),
            new LoadLinkNestedLinkedSourceByNullableIdWhenIdSuffixMatches(),
            new LoadLinkReferenceWhenIdSuffixMatches(),
            new LoadLinkNestedLinkedSourceWhenIdSuffixMatches(),
            new LoadLinkReferenceListWhenIdSuffixMatches(),
            new LoadLinkNestedLinkedSourceListWhenIdSuffixMatches(),
            new LoadLinkNestedLinkedSourceFromModelWhenNameMatches(),
            new LoadLinkNestedLinkedSourceListFromModelWhenNameMatches()
        };

        /// <summary>
        /// Get the default LinkIt conventions and some custom conventions.
        /// </summary>
        public static List<ILoadLinkExpressionConvention> DefaultAnd(params ILoadLinkExpressionConvention[] customConventions)
        {
            if (customConventions == null)
            {
                throw new ArgumentNullException(nameof(customConventions));
            }

            return Default
                .Concat(customConventions)
                .ToList();
        }
    }
}