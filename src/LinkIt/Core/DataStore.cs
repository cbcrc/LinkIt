// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Shared;

namespace LinkIt.Core
{
    internal class DataStore
    {
        private readonly Dictionary<Type, IDictionary> _loadedReferencesByType = new Dictionary<Type, IDictionary>();

        public IEnumerable<object> GetLoadedReferenceIds(Type referenceType)
        {
            if (!_loadedReferencesByType.ContainsKey(referenceType))
            {
                return Enumerable.Empty<object>();
            }

            return _loadedReferencesByType[referenceType].Keys.Cast<object>();
        }

        private Dictionary<TId, TReference> GetReferenceDictionary<TReference, TId>()
        {
            var tReference = typeof(TReference);
            if (!_loadedReferencesByType.ContainsKey(tReference))
            {
                return null;
            }

            return _loadedReferencesByType[tReference] as Dictionary<TId, TReference>;
        }

        public void AddReferences<TReference, TId>(IDictionary<TId, TReference> referencesById)
        {
            if (referencesById == null)
            {
                throw new ArgumentNullException(nameof(referencesById));
            }

            var dictionary = GetReferenceDictionary<TReference, TId>() ?? new Dictionary<TId, TReference>();
            foreach (var reference in referencesById)
            {
                dictionary[reference.Key] = reference.Value;
            }

            _loadedReferencesByType[typeof(TReference)] = dictionary;
        }

        public TReference GetReference<TReference, TId>(TId lookupId)
        {
            if (lookupId.EqualsDefaultValue())
            {
                return default;
            }

            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();
            return referenceDictionnary.GetValueOrDefault(lookupId);
        }

        public IReadOnlyList<TReference> GetReferences<TReference, TId>(IEnumerable<TId> lookupIds)
        {
            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();
            return lookupIds
                .Select(id => referenceDictionnary.GetValueOrDefault(id))
                .ToList();
        }
    }
}
