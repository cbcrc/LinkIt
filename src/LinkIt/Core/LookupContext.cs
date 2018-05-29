// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Shared;

namespace LinkIt.Core
{
    /// <summary>
    ///     Responsible for gathering the lookup ids of a loading level.
    /// </summary>
    internal class LookupContext
    {
        private readonly Dictionary<Type, HashSet<object>> _lookupIdsByReferenceType = new Dictionary<Type, HashSet<object>>();

        public IReadOnlyDictionary<Type, IReadOnlyList<object>> LookupIds
            => _lookupIdsByReferenceType.ToDictionary(p => p.Key, p => (IReadOnlyList<object>) p.Value.ToList());

        public void AddLookupId<TReference, TId>(TId lookupId)
        {
            if (lookupId.EqualsDefaultValue())
            {
                return;
            }

            AddLookupIds(typeof(TReference), new[] { lookupId });
        }

        public void AddLookupIds<TReference, TId>(IEnumerable<TId> lookupIds)
        {
            if (lookupIds == null)
            {
                throw new ArgumentNullException(nameof(lookupIds));
            }

            AddLookupIds(typeof(TReference), lookupIds);
        }

        private void AddLookupIds<TId>(Type referenceType, IEnumerable<TId> lookupIds)
        {
            var nonNullIds = lookupIds
                .Where(id => !id.EqualsDefaultValue())
                .ToList();
            if (nonNullIds.Count == 0)
            {
                return;
            }

            var currentLookupIds = GetOrAddLookupIdsFor(referenceType);
            foreach (var id in nonNullIds)
            {
                currentLookupIds.Add(id);
            }
        }

        private HashSet<object> GetOrAddLookupIdsFor(Type referenceType)
        {
            if (_lookupIdsByReferenceType.ContainsKey(referenceType))
            {
                return _lookupIdsByReferenceType[referenceType];
            }

            var emptySet = new HashSet<object>();
            _lookupIdsByReferenceType.Add(referenceType, emptySet);
            return emptySet;
        }
    }
}
