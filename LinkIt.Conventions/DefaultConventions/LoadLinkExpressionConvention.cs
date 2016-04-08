using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Conventions.Interfaces;

namespace LinkIt.Conventions.DefaultConventions
{
    public static class LoadLinkExpressionConvention
    {
        public static List<ILoadLinkExpressionConvention> Default{
            get
            {
                return new List<ILoadLinkExpressionConvention>
                {
                    new LoadLinkByNullableValueTypeIdWhenIdSuffixMatches(),
                    new LoadLinkMultiValueWhenIdSuffixMatches(),
                    new LoadLinkSingleValueWhenIdSuffixMatches(),
                    new LoadLinkMultiValueNestedLinkedSourceFromModelWhenNameMatches(),
                    new LoadLinkSingleValueNestedLinkedSourceFromModelWhenNameMatches(),
                };
            }
        }

        public static List<ILoadLinkExpressionConvention> DefaultAnd(params ILoadLinkExpressionConvention[] customConventions) {
            if (customConventions == null) { throw new ArgumentNullException("customConventions"); }

            return Default
                .Concat(customConventions)
                .ToList();
        }

    }
}