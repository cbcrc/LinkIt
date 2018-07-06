// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Debugging;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    internal class LoadingContext : ILoadingContext
    {
        private readonly DataStore _dataStore;
        private readonly IReadOnlyDictionary<Type, IReadOnlyList<object>> _lookupIds;

        public LoadingContext(IReadOnlyDictionary<Type, IReadOnlyList<object>> lookupIds, DataStore dataStore, ILoadLinkDetails loadLinkDetails = null)
        {
            _dataStore = dataStore;
            _lookupIds = lookupIds;
            LoadLinkDetails = loadLinkDetails;
        }

        public ILoadLinkDetails LoadLinkDetails { get; }

        public IReadOnlyList<Type> ReferenceTypes => _lookupIds.Keys.ToList();

        public IReadOnlyDictionary<Type, IReadOnlyList<object>> ReferenceIds()
            => _lookupIds;

        public IReadOnlyList<TId> ReferenceIds<TReference, TId>()
            => ReferenceIds<TId>(typeof(TReference));

        public IReadOnlyList<TId> ReferenceIds<TId>(Type referenceType)
            => GetLookupIds(referenceType).OfType<TId>().ToList();

        private IReadOnlyList<object> GetLookupIds(Type referenceType)
        {
            if (referenceType is null)
            {
                throw new ArgumentNullException(nameof(referenceType));
            }

            return _lookupIds.TryGetValue(referenceType, out var lookupIds) ? lookupIds : new object[0];
        }

        public void AddResults<TReference, TId>(IEnumerable<TReference> references, Func<TReference, TId> getReferenceId)
        {
            if (references == null)
            {
                throw new ArgumentNullException(nameof(references));
            }

            if (getReferenceId == null)
            {
                throw new ArgumentNullException(nameof(getReferenceId));
            }

            var referenceDictionary = references
                .Select(reference => new KeyValuePair<TId, TReference>(getReferenceId(reference), reference));

            _dataStore.AddReferences(referenceDictionary);
            LoadLinkDetails?.CurrentStep.AddReferenceValues(references);
        }

        public void AddResults<TReference, TId>(IDictionary<TId, TReference> referencesById)
        {
            if (referencesById == null)
            {
                throw new ArgumentNullException(nameof(referencesById));
            }

            _dataStore.AddReferences(referencesById);
            LoadLinkDetails?.CurrentStep.AddReferenceValues(referencesById.Values);
        }
    }
}
