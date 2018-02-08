// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.PublicApi;

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
            if (id == null)
            {
                return default;
            }

            var models = await ByIdsAsync(new [] { id });
            return models.SingleOrDefault();
        }

        public async Task<IReadOnlyList<TModel>> ByIdsAsync<TId>(IEnumerable<TId> ids)
        {
            var lookupContext = new LookupContext();
            lookupContext.AddLookupIds<TModel, TId>(ids);

            if (!lookupContext.LookupIds.Any())
            {
                return new TModel[0];
            }

            var dataStore = new DataStore();
            var loadingContext = new LoadingContext(lookupContext, dataStore);

            using (var referenceLoader = _createReferenceLoader())
            {
                await referenceLoader.LoadReferencesAsync(loadingContext);
            }

            return dataStore.GetReferences<TModel, TId>(ids)
                .Where(m => m != null)
                .ToList();
        }
    }
}