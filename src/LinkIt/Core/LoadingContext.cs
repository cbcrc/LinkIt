// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    /// <summary>
    /// In addition to the responsiblies of ILoadingContext,
    /// responsible for gathering the lookup ids of a loading level.
    /// </summary>
    internal class LoadingContext : ILoadingContext
    {
        private readonly Linker _linker;
        private readonly Dictionary<Type, HashSet<object>> _lookupIdsByReferenceType = new Dictionary<Type, HashSet<object>>();

        public LoadingContext(Linker linker)
        {
            _linker = linker;
        }

        public IReadOnlyList<Type> ReferenceTypes => _lookupIdsByReferenceType.Keys.ToList();

        public IReadOnlyDictionary<Type, IReadOnlyList<object>> ReferenceIds()
            => _lookupIdsByReferenceType.ToDictionary(p => p.Key, p => (IReadOnlyList<object>) p.Value.ToList());

        public IReadOnlyList<TId> ReferenceIds<TReference, TId>()
        {
            return ReferenceIds<TId>(typeof(TReference));
        }

        public IReadOnlyList<TId> ReferenceIds<TId>(Type referenceType)
        {
            return GetLookupIds(referenceType)?.OfType<TId>().ToArray() ?? new TId[0];
        }

        private HashSet<object> GetLookupIds(Type referenceType)
        {
            return _lookupIdsByReferenceType.ContainsKey(referenceType)
                ? _lookupIdsByReferenceType[referenceType]
                : null;
        }

        public void AddResults<TReference, TId>(IEnumerable<TReference> references, Func<TReference, TId> getReferenceId)
        {
            if (references == null) throw new ArgumentNullException(nameof(references));
            if (getReferenceId == null) throw new ArgumentNullException(nameof(getReferenceId));

            var referenceDictionary = references.ToDictionary(
                getReferenceId,
                reference => reference
            );

            _linker.AddReferences(referenceDictionary);
        }

        public void AddResults<TReference, TId>(IDictionary<TId, TReference> referencesById)
        {
            _linker.AddReferences(referencesById);
        }

        public void AddLookupId<TReference>(object lookupId)
        {
            AddLookupId(typeof(TReference), lookupId);
        }

        private void AddLookupId(Type referenceType, object lookupId)
        {
            if (lookupId == null) return;

            var currentLookupIds = GetOrAddLookupIdsFor(referenceType);
            currentLookupIds.Add(lookupId);
        }

        private HashSet<object> GetOrAddLookupIdsFor(Type referenceType)
        {
            var lookupIds = GetLookupIds(referenceType);
            if (lookupIds != null)
            {
                return lookupIds;
            }

            var set = new HashSet<object>();
            _lookupIdsByReferenceType.Add(referenceType, set);
            return set;
        }

        public void AddLookupIds<TReference, TId>(IEnumerable<TId> lookupIds)
        {
            AddLookupIds(typeof(TReference), lookupIds);
        }

        private void AddLookupIds<TId>(Type referenceType, IEnumerable<TId> lookupIds)
        {
            if (lookupIds == null)
            {
                throw new ArgumentNullException(nameof(lookupIds));
            }

            var nonNullIds = lookupIds.Where(id => id != null).ToList();
            if (!nonNullIds.Any())
            {
                return;
            }

            var currentLookupIds = GetOrAddLookupIdsFor(referenceType);
            foreach (var id in nonNullIds)
            {
                currentLookupIds.Add(id);
            }
        }
    }
}