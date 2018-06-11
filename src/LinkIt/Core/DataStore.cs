// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        private ImmutableDictionary<TId, TReference> GetReferenceDictionary<TReference, TId>()
        {
            if (!_loadedReferencesByType.TryGetValue(typeof(TReference), out var dictionary))
            {
                return null;
            }

            return dictionary as ImmutableDictionary<TId, TReference>;
        }

        public void AddReferences<TReference, TId>(IEnumerable<KeyValuePair<TId, TReference>> referencesById)
        {
            _loadedReferencesByType.AddOrUpdate(
                typeof(TReference),
                _ => ImmutableDictionary.CreateRange(referencesById),
                (_, dict) => (dict as ImmutableDictionary<TId, TReference>)?.AddRange(referencesById)
            );
        }

        public TReference GetReference<TReference, TId>(TId lookupId)
        {
            if (lookupId.EqualsDefaultValue())
            {
                return default;
            }

            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();
            if (referenceDictionnary == null)
            {
                return default;
            }

            return referenceDictionnary.GetValueOrDefault(lookupId);
        }

        public IReadOnlyList<TReference> GetReferences<TReference, TId>(IEnumerable<TId> lookupIds)
        {
            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();
            if (referenceDictionnary == null)
            {
                return new TReference[0];
            }

            return lookupIds
                .Where(id => !id.EqualsDefaultValue())
                .Select(id => referenceDictionnary.GetValueOrDefault(id))
                .ToList();
        }
    }
}
