// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.Shared;

namespace LinkIt.Core.Includes
{
    /// <summary>
    /// Responsible for giving access to the includes of a specific link target.
    /// </summary>
    internal class IncludeSet<TLinkedSource, TAbstractChildLinkedSource, TLink, TDiscriminant>
    {
        private readonly Func<TLink, TDiscriminant> _getDiscriminant;
        private readonly Dictionary<TDiscriminant, IInclude> _includes;

        public IncludeSet(Dictionary<TDiscriminant, IInclude> includes, Func<TLink, TDiscriminant> getDiscriminant)
        {
            _includes = includes;
            _getDiscriminant = getDiscriminant;
        }

        public IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink> GetIncludeWithCreateNestedLinkedSourceByIdForReferenceType(TLink link, Type referenceType)
        {
            var include = GetInclude<IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink>>(link);

            if (include == null || include.ReferenceType != referenceType)
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

        public List<IIncludeWithAddLookupId<TLink>> GetIncludesWithAddLookupId()
        {
            return GetIncludes<IIncludeWithAddLookupId<TLink>>();
        }

        public List<IIncludeWithChildLinkedSource> GetIncludesWithChildLinkedSource()
        {
            return GetIncludes<IIncludeWithChildLinkedSource>();
        }

        private TInclude GetInclude<TInclude>(TLink link)
            where TInclude : class
        {
            AssumeNotNullLink(link);

            var discriminant = _getDiscriminant(link);
            AssumeIncludeExistsForDiscriminant(discriminant);

            var include = _includes[discriminant];

            return include as TInclude;
        }

        private void AssumeIncludeExistsForDiscriminant(TDiscriminant discriminant)
        {
            if (!_includes.ContainsKey(discriminant))
            {
                throw new LinkItException(
                    $"{typeof(TLinkedSource)}: Cannot invoke GetInclude for discriminant={discriminant}"
                );
            }
        }

        private static void AssumeNotNullLink(TLink link)
        {
            if (link.EqualsDefaultValue())
            {
                throw new LinkItException(
                    $"{typeof(TLinkedSource)}: Cannot invoke GetInclude with a null link"
                );
            }
        }

        public List<TInclude> GetIncludes<TInclude>()
            where TInclude : class, IInclude
        {
            return _includes.Values
                .OfType<TInclude>()
                .ToList();
        }
    }
}