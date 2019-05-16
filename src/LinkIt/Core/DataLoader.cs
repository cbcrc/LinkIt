// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.PublicApi;
using LinkIt.Shared;

namespace LinkIt.Core
{
    internal class DataLoader<TModel> : IDataLoader<TModel>
    {
        private readonly Func<IReferenceLoader> _createReferenceLoader;

        public DataLoader(Func<IReferenceLoader> createReferenceLoader)
        {
            _createReferenceLoader = createReferenceLoader;
        }

        public async Task<TModel> ByIdAsync<TId>(TId id)
        {
            if (id is null)
            {
                return default;
            }

            var models = await ByIdsAsync(new [] { id }).ConfigureAwait(false);
            return models.SingleOrDefault();
        }

        public async Task<IReadOnlyList<TModel>> ByIdsAsync<TId>(IEnumerable<TId> ids)
        {
            var lookupContext = new LookupContext();
            lookupContext.AddLookupIds<TModel, TId>(ids);

            if (lookupContext.LookupIds.Count == 0)
            {
                return new TModel[0];
            }

            var dataStore = new DataStore();
            var loadingContext = new LoadingContext(lookupContext.LookupIds, dataStore);

            using (var referenceLoader = _createReferenceLoader())
            {
                await referenceLoader.LoadReferencesAsync(loadingContext).ConfigureAwait(false);
            }

            return dataStore.GetReferences<TModel, TId>(ids)
                .WhereNotNull()
                .ToList();
        }
    }
}
