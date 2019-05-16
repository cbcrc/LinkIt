// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Shared;

namespace LinkIt.Core
{
    internal class DataStore
    {
        private readonly ConcurrentDictionary<Type, IDictionary> _loadedReferencesByType = new ConcurrentDictionary<Type, IDictionary>();

        public IReadOnlyList<object> GetLoadedReferenceIds(Type referenceType)
        {
            if (!_loadedReferencesByType.TryGetValue(referenceType, out var references))
            {
                return new object[0];
            }

            return references.Keys
                .Cast<object>()
                .ToArray();
        }

        private Dictionary<TId, TReference> GetReferenceDictionary<TReference, TId>()
        {
            if (!_loadedReferencesByType.TryGetValue(typeof(TReference), out var dictionary))
            {
                return null;
            }

            return dictionary as Dictionary<TId, TReference>;
        }

        public void AddReferences<TReference, TId>(IEnumerable<KeyValuePair<TId, TReference>> referencesById)
        {
            _loadedReferencesByType.AddOrUpdate(
                typeof(TReference),
                _ => referencesById.ToDictionary(p => p.Key, p => p.Value),
                (_, dict) => (dict as Dictionary<TId, TReference>)?.AddRange(referencesById)
            );
        }

        public TReference GetReference<TReference, TId>(TId lookupId)
        {
            if (lookupId is null)
            {
                return default;
            }

            var referenceDictionary = GetReferenceDictionary<TReference, TId>();
            return referenceDictionary is null ? default : referenceDictionary.GetValueOrDefault(lookupId);
        }

        public IReadOnlyList<TReference> GetReferences<TReference, TId>(IEnumerable<TId> lookupIds)
        {
            var referenceDictionary = GetReferenceDictionary<TReference, TId>();
            if (referenceDictionary is null)
            {
                return new TReference[0];
            }

            return lookupIds
                .WhereNotNull()
                .Select(id => referenceDictionary.GetValueOrDefault(id))
                .ToList();
        }
    }
}
