// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.ReadableExpressions.Extensions;
using LinkIt.Shared;

namespace LinkIt.Core.Includes
{
    /// <summary>
    /// Responsible for giving access to the includes of a specific link target.
    /// </summary>
    internal class IncludeSet<TLinkedSource, TAbstractChildLinkedSource, TLink, TDiscriminant>
    {
        private readonly Func<TLink, TDiscriminant> _getDiscriminant;
        private readonly bool _ignoreUnhandledCases;
        private readonly Dictionary<TDiscriminant, IInclude> _includesByDiscriminant;
        private readonly List<IInclude> _includes;

        public IncludeSet(Dictionary<TDiscriminant, IInclude> includes, Func<TLink, TDiscriminant> getDiscriminant, bool ignoreUnhandledCases)
        {
            _includesByDiscriminant = includes;
            _includes = includes.Values.ToList();
            _getDiscriminant = getDiscriminant;
            _ignoreUnhandledCases = ignoreUnhandledCases;
        }

        public IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink> GetIncludeWithCreateNestedLinkedSourceByIdForReferenceType(TLink link, Type referenceType)
        {
            var include = GetInclude<IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink>>(link);

            if (include is null || include.ReferenceType != referenceType)
            {
                return null;
            }

            return include;
        }

        public IIncludeWithCreateNestedLinkedSourceFromModel<TLinkedSource, TAbstractChildLinkedSource, TLink> GetIncludeWithCreateNestedLinkedSourceFromModel(TLink link)
        {
            return GetInclude<IIncludeWithCreateNestedLinkedSourceFromModel<TLinkedSource, TAbstractChildLinkedSource, TLink>>(link);
        }

        public IIncludeWithAddLookupId<TLink> GetIncludeWithAddLookupId(TLink linkForReferenceType)
        {
            return GetInclude<IIncludeWithAddLookupId<TLink>>(linkForReferenceType);
        }

        public IIncludeWithGetReference<TAbstractChildLinkedSource, TLink> GetIncludeWithGetReference(TLink link)
        {
            return GetInclude<IIncludeWithGetReference<TAbstractChildLinkedSource, TLink>>(link);
        }

        public IEnumerable<IIncludeWithAddLookupId<TLink>> GetIncludesWithAddLookupId()
        {
            return GetIncludes<IIncludeWithAddLookupId<TLink>>();
        }

        public IEnumerable<IIncludeWithChildLinkedSource> GetIncludesWithChildLinkedSource()
        {
            return GetIncludes<IIncludeWithChildLinkedSource>();
        }

        private TInclude GetInclude<TInclude>(TLink link)
            where TInclude : class
        {
            AssumeNotNullLink(link);

            var discriminant = _getDiscriminant(link);

            _includesByDiscriminant.TryGetValue(discriminant, out var include);
            if (include != null)
            {
                return include as TInclude;
            }

            if (_ignoreUnhandledCases)
            {
                return null;
            }

            throw new LinkItException(
                $"{typeof(TLinkedSource).GetFriendlyName()}: Cannot invoke GetInclude for discriminant={discriminant}"
            );
        }

        private static void AssumeNotNullLink(TLink link)
        {
            if ((object) link == null)
            {
                throw new LinkItException(
                    $"{typeof(TLinkedSource).GetFriendlyName()}: Cannot invoke GetInclude with a null link"
                );
            }
        }

        public IEnumerable<TInclude> GetIncludes<TInclude>()
            where TInclude : class, IInclude
        {
            return _includes.OfType<TInclude>();
        }
    }
}
