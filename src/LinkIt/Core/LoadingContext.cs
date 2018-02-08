// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    internal class LoadingContext : ILoadingContext
    {
        private readonly DataStore _dataStore;
        private readonly IReadOnlyDictionary<Type, IReadOnlyList<object>> _lookupIds;

        public LoadingContext(LookupContext lookupContext, DataStore dataStore)
        {
            _lookupIds = lookupContext.LookupIds;
            _dataStore = dataStore;
        }

        public IReadOnlyList<Type> ReferenceTypes => _lookupIds.Keys.ToList();

        public IReadOnlyDictionary<Type, IReadOnlyList<object>> ReferenceIds()
            => _lookupIds;

        public IReadOnlyList<TId> ReferenceIds<TReference, TId>()
            => ReferenceIds<TId>(typeof(TReference));

        public IReadOnlyList<TId> ReferenceIds<TId>(Type referenceType)
            => GetLookupIds(referenceType).OfType<TId>().ToList();

        private IReadOnlyList<object> GetLookupIds(Type referenceType)
            => _lookupIds.ContainsKey(referenceType) ? _lookupIds[referenceType] : new object[0];

        public void AddResults<TReference, TId>(IEnumerable<TReference> references, Func<TReference, TId> getReferenceId)
        {
            if (references == null) throw new ArgumentNullException(nameof(references));
            if (getReferenceId == null) throw new ArgumentNullException(nameof(getReferenceId));

            var referenceDictionary = references.ToDictionary(
                getReferenceId,
                reference => reference
            );

            _dataStore.AddReferences(referenceDictionary);
        }

        public void AddResults<TReference, TId>(IDictionary<TId, TReference> referencesById)
        {
            _dataStore.AddReferences(referencesById);
        }
    }
}