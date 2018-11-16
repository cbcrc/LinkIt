// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.Diagnostics;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    internal class LoadLinker<TRootLinkedSource, TRootLinkedSourceModel> : IDisposable
        where TRootLinkedSource : class, ILinkedSource<TRootLinkedSourceModel>, new()
    {
        private readonly LoadLinkProtocol _loadLinkProtocol;
        private readonly IReferenceLoader _referenceLoader;
        private readonly IReadOnlyList<IReadOnlyList<Type>> _referenceTypesToBeLoadedForEachLoadingLevel;
        private readonly DataStore _dataStore = new DataStore();
        private readonly Linker _linker;
        private readonly LoadLinkDetails<TRootLinkedSource, TRootLinkedSourceModel> _loadLinkDetails;

        internal LoadLinker(Func<IReferenceLoader> getReferenceLoader, IReadOnlyList<IReadOnlyList<Type>> referenceTypesToBeLoadedForEachLoadingLevel, LoadLinkProtocol loadLinkProtocol, LoadLinkDetails<TRootLinkedSource, TRootLinkedSourceModel> loadLinkDetails)
        {
            _referenceLoader = getReferenceLoader();
            _referenceTypesToBeLoadedForEachLoadingLevel = referenceTypesToBeLoadedForEachLoadingLevel;
            _loadLinkProtocol = loadLinkProtocol;
            _loadLinkDetails = loadLinkDetails;
            _linker = new Linker(_loadLinkProtocol, _dataStore);
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> FromModelsAsync(
            IEnumerable<TRootLinkedSourceModel> models,
            Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            var linkedSources = CreatePartiallyBuiltLinkedSource(models, initRootLinkedSources);

            await LoadLinkRootLinkedSource().ConfigureAwait(false);

            return linkedSources;
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> ByIdsAsync<TRootLinkedSourceModelId>(
            IEnumerable<TRootLinkedSourceModelId> modelIds,
            Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            var models = await LoadRootLinkedSourceModelAsync(modelIds).ConfigureAwait(false);
            return await FromModelsAsync(
                models,
                initRootLinkedSources
            ).ConfigureAwait(false);
        }

        private IReadOnlyList<TRootLinkedSource> CreatePartiallyBuiltLinkedSource(IEnumerable<TRootLinkedSourceModel> models, Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            _loadLinkDetails?.CurrentStep.LinkStart();

            var linkedSources = models
                .Select((model, index) => CreateLinkedSource(model, index, initRootLinkedSources))
                .Where(linkedSource => linkedSource != null)
                .ToList();

            _loadLinkDetails?.CurrentStep.LinkEnd();

            _loadLinkDetails?.SetResult(linkedSources);

            return linkedSources;
        }

        private TRootLinkedSource CreateLinkedSource(TRootLinkedSourceModel model, int index, Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            return _linker.CreatePartiallyBuiltLinkedSource<TRootLinkedSource, TRootLinkedSourceModel>(
                model,
                childLinkedSource => initRootLinkedSources?.Invoke(index, childLinkedSource)
            );
        }

        private async Task<IEnumerable<TRootLinkedSourceModel>> LoadRootLinkedSourceModelAsync<TRootLinkedSourceModelId>(IEnumerable<TRootLinkedSourceModelId> modelIds)
        {
            var lookupContext = new LookupContext();
            lookupContext.AddLookupIds<TRootLinkedSourceModel, TRootLinkedSourceModelId>(modelIds);

            var loadingContext = GetLoadingContext(lookupContext);

            _loadLinkDetails?.CurrentStep.LoadStart();
            await _referenceLoader.LoadReferencesAsync(loadingContext).ConfigureAwait(false);
            _loadLinkDetails?.CurrentStep.LoadEnd();

            return _dataStore.GetReferences<TRootLinkedSourceModel, TRootLinkedSourceModelId>(modelIds);
        }

        private LoadingContext GetLoadingContext(LookupContext lookupContext)
        {
            var lookupIds = GetLookupIdsToLoad(lookupContext);
            _loadLinkDetails?.CurrentStep.SetReferenceIds(lookupIds);
            return new LoadingContext(lookupIds, _dataStore, _loadLinkDetails);
        }

        private IReadOnlyDictionary<Type, IReadOnlyList<object>> GetLookupIdsToLoad(LookupContext lookupContext)
        {
            return lookupContext.LookupIds
                .Select(p => new {
                    Type = p.Key,
                    Ids = p.Value.Except(_dataStore.GetLoadedReferenceIds(p.Key)).ToList(),
                })
                .Where(p => p.Ids.Count > 0)
                .ToDictionary(
                    p => p.Type,
                    p => (IReadOnlyList<object>) p.Ids
                );
        }

        private async Task LoadLinkRootLinkedSource()
        {
            await LoadAsync().ConfigureAwait(false);

            _linker.LinkReferences();
        }

        private async Task LoadAsync()
        {
            // root model is already loaded, so we can skip that level
            foreach (var referenceTypesToBeLoaded in _referenceTypesToBeLoadedForEachLoadingLevel.Skip(1))
            {
                _loadLinkDetails?.NextStep();
                await LoadNestingLevelAsync(referenceTypesToBeLoaded).ConfigureAwait(false);

                _loadLinkDetails?.CurrentStep.LinkStart();
                _linker.LinkNestedLinkedSourcesById(referenceTypesToBeLoaded);
                _loadLinkDetails?.CurrentStep.LinkEnd();
            }
        }

        private async Task LoadNestingLevelAsync(IReadOnlyList<Type> referenceTypesToBeLoaded)
        {
            var lookupContext = GetLookupContextForLoadingLevel(referenceTypesToBeLoaded);
            var loadingContext = GetLoadingContext(lookupContext);

            _loadLinkDetails?.CurrentStep.LoadStart();
            if (loadingContext.ReferenceTypes.Count > 0)
            {
                await _referenceLoader.LoadReferencesAsync(loadingContext).ConfigureAwait(false);
            }
            _loadLinkDetails?.CurrentStep.LoadEnd();
        }

        private LookupContext GetLookupContextForLoadingLevel(IReadOnlyList<Type> referenceTypesToBeLoaded)
        {
            var lookupContext = new LookupContext();
            foreach (var linkedSource in _linker.LinkedSourcesToBeBuilt)
            {
                AddLookupIdsForLinkedSource(lookupContext, linkedSource, referenceTypesToBeLoaded);
            }

            return lookupContext;
        }

        private void AddLookupIdsForLinkedSource(LookupContext lookupContext, object linkedSource, IReadOnlyList<Type> referenceTypesToBeLoaded)
        {
            var loadLinkExpressionsForLinkedSource = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource);
            foreach (var referenceType in referenceTypesToBeLoaded)
            {
                var loadLinkExpressionsForReference = loadLinkExpressionsForLinkedSource.Where(e => e.ReferenceTypes.Contains(referenceType));
                foreach (var loadLinkExpression in loadLinkExpressionsForReference)
                {
                    loadLinkExpression.AddLookupIds(linkedSource, lookupContext, referenceType);
                }
            }
        }

        public void Dispose()
        {
            _referenceLoader?.Dispose();
        }
    }
}
