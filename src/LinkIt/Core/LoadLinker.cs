// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    /// <inheritdoc/>
    internal class LoadLinker<TRootLinkedSource, TRootLinkedSourceModel> : ILoadLinker<TRootLinkedSource>
        where TRootLinkedSource : class, ILinkedSource<TRootLinkedSourceModel>, new()
    {
        private readonly LoadLinkProtocol _loadLinkProtocol;
        private readonly IReferenceLoader _referenceLoader;
        private readonly List<List<Type>> _referenceTypesToBeLoadedForEachLoadingLevel;
        private readonly DataStore _dataStore = new DataStore();
        private readonly Linker _linker;

        internal LoadLinker(IReferenceLoader referenceLoader, List<List<Type>> referenceTypesToBeLoadedForEachLoadingLevel, LoadLinkProtocol loadLinkProtocol)
        {
            _referenceLoader = referenceLoader;
            _referenceTypesToBeLoadedForEachLoadingLevel = referenceTypesToBeLoadedForEachLoadingLevel;
            _loadLinkProtocol = loadLinkProtocol;
            _linker = new Linker(_loadLinkProtocol, _dataStore);
        }

        public async Task<TRootLinkedSource> FromModelAsync<TModel>(
            TModel model,
            Action<TRootLinkedSource> initRootLinkedSource)
        {
            var linkedSources = await FromModelsAsync(new [] { model }, (_, linkedSource) => initRootLinkedSource?.Invoke(linkedSource));
            return linkedSources.SingleOrDefault();
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> FromModelsAsync<TModel>(
            IEnumerable<TModel> models,
            Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            var linkedSources = models
                .Cast<TRootLinkedSourceModel>()
                .Select((model, index) => CreateLinkedSource(model, index, initRootLinkedSources))
                .ToList();

            await LoadLinkRootLinkedSource();

            return linkedSources
                .Where(linkedSource => linkedSource != null)
                .ToList();
        }


        public async Task<TRootLinkedSource> ByIdAsync<TRootLinkedSourceModelId>(
            TRootLinkedSourceModelId modelId,
            Action<TRootLinkedSource> initRootLinkedSource)
        {
            var linkedSources = await ByIdsAsync(new [] { modelId }, (_, linkedSource) => initRootLinkedSource?.Invoke(linkedSource));
            return linkedSources.SingleOrDefault();
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> ByIdsAsync<TRootLinkedSourceModelId>(
            IEnumerable<TRootLinkedSourceModelId> modelIds,
            Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            var models = await LoadRootLinkedSourceModelAsync(modelIds);
            return await FromModelsAsync(
                models,
                initRootLinkedSources
            );
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

            await _referenceLoader.LoadReferencesAsync(new LoadingContext(lookupContext, _dataStore));

            return _dataStore.GetReferences<TRootLinkedSourceModel, TRootLinkedSourceModelId>(modelIds);
        }

        private async Task LoadLinkRootLinkedSource()
        {
            await LoadAsync();

            _linker.LinkReferences();
        }

        private async Task LoadAsync()
        {
            foreach (var referenceTypesToBeLoaded in _referenceTypesToBeLoadedForEachLoadingLevel)
            {
                await LoadNestingLevelAsync(referenceTypesToBeLoaded);
                _linker.LinkNestedLinkedSourcesById(referenceTypesToBeLoaded);
            }
        }

        private async Task LoadNestingLevelAsync(List<Type> referenceTypeToBeLoaded)
        {
            var lookupContext = GetLookupContextForLoadingLevel(referenceTypeToBeLoaded);
            var loadingContext = new LoadingContext(lookupContext, _dataStore);
            await _referenceLoader.LoadReferencesAsync(loadingContext);
        }

        private LookupContext GetLookupContextForLoadingLevel(List<Type> referenceTypes)
        {
            var lookupContext = new LookupContext();

            foreach (var referenceType in referenceTypes)
            {
                foreach (var linkedSource in _linker.LinkedSourcesToBeBuilt)
                {
                    var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource, referenceType);
                    foreach (var loadLinkExpression in loadLinkExpressions)
                    {
                        loadLinkExpression.AddLookupIds(linkedSource, lookupContext, referenceType);
                    }
                }
            }

            return lookupContext;
        }
    }
}