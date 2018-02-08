using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinkIt.PublicApi;
using LinkIt.Shared;

namespace LinkIt.Core
{
    internal class DataStore
    {
        private readonly Dictionary<Type, IDictionary> _loadedReferencesByType = new Dictionary<Type, IDictionary>();

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
            if (referencesById == null) throw new ArgumentNullException(nameof(referencesById));

            var dictionary = GetReferenceDictionary<TReference, TId>() ?? new Dictionary<TId, TReference>();
            foreach (var reference in referencesById)
            {
                dictionary[reference.Key] = reference.Value;
            }

            _loadedReferencesByType[typeof(TReference)] = dictionary;
        }

        public TReference GetReference<TReference, TId>(TId lookupId)
        {
            if (lookupId == null) return default;

            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();
            return referenceDictionnary.GetValueOrDefault(lookupId);
        }

        public IReadOnlyList<TReference> GetReferences<TReference,TId>(IEnumerable<TId> lookupIds)
        {
            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();
            return lookupIds
                .Select(id => referenceDictionnary.GetValueOrDefault(id))
                .ToList();
        }
    }
}