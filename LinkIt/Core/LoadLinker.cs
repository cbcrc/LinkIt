#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

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
        private readonly Linker _linker = new Linker();

        internal LoadLinker(IReferenceLoader referenceLoader, List<List<Type>> referenceTypesToBeLoadedForEachLoadingLevel, LoadLinkProtocol loadLinkProtocol)
        {
            _referenceLoader = referenceLoader;
            _referenceTypesToBeLoadedForEachLoadingLevel = referenceTypesToBeLoadedForEachLoadingLevel;
            _loadLinkProtocol = loadLinkProtocol;
        }

        public async Task<TRootLinkedSource> FromModelAsync<TModel>(
            TModel model,
            Action<TRootLinkedSource> initRootLinkedSource)
        {
            var linkedSources = await FromModelsAsync(
                new [] { model },
                ToInitRootLinkedSources(initRootLinkedSource)
            );
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
            var linkedSources = await ByIdsAsync(
                new List<TRootLinkedSourceModelId> { modelId },
                ToInitRootLinkedSources(initRootLinkedSource)
            );
            return linkedSources.SingleOrDefault();
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> ByIdsAsync<TRootLinkedSourceModelId>(
            IEnumerable<TRootLinkedSourceModelId> modelIds,
            Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            var models = await LoadRootLinkedSourceModelAsync(modelIds.ToList());
            return await FromModelsAsync(
                models,
                initRootLinkedSources
            );
        }

        private TRootLinkedSource CreateLinkedSource(TRootLinkedSourceModel model, int index, Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            return _linker.CreatePartiallyBuiltLinkedSource<TRootLinkedSource, TRootLinkedSourceModel>(
                model,
                _loadLinkProtocol,
                childLinkedSource => initRootLinkedSources?.Invoke(index, childLinkedSource)
            );
        }

        private static Action<int, TRootLinkedSource> ToInitRootLinkedSources(Action<TRootLinkedSource> initRootLinkedSource)
        {
            return (referenceIndex, rootLinkedSource) => initRootLinkedSource?.Invoke(rootLinkedSource);
        }

        private async Task<IEnumerable<TRootLinkedSourceModel>> LoadRootLinkedSourceModelAsync<TRootLinkedSourceModelId>(List<TRootLinkedSourceModelId> modelIds)
        {
            var loadingContext = new LoadingContext(_linker);
            loadingContext.AddMulti<TRootLinkedSourceModel, TRootLinkedSourceModelId>(modelIds);

            await _referenceLoader.LoadReferencesAsync(loadingContext);

            return _linker.GetOptionalReferences<TRootLinkedSourceModel, TRootLinkedSourceModelId>(modelIds);
        }

        private async Task LoadLinkRootLinkedSource()
        {
            await LoadAsync();

            LinkReferences();

            FilterOutNullValues();
        }

        private async Task LoadAsync()
        {
            foreach (var referenceTypesToBeLoaded in _referenceTypesToBeLoadedForEachLoadingLevel)
            {
                await LoadNestingLevelAsync(referenceTypesToBeLoaded);
                LinkNestedLinkedSourcesById(referenceTypesToBeLoaded);
            }
        }

        private async Task LoadNestingLevelAsync(List<Type> referenceTypeToBeLoaded)
        {
            var loadingContext = GetLoadingContextForLoadingLevel(referenceTypeToBeLoaded);
            await _referenceLoader.LoadReferencesAsync(loadingContext);
        }

        private LoadingContext GetLoadingContextForLoadingLevel(List<Type> referenceTypes)
        {
            var loadingContext = new LoadingContext(_linker);

            foreach (var referenceType in referenceTypes)
            {
                foreach (var linkedSource in _linker.LinkedSourcesToBeBuilt)
                {
                    var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource, referenceType);
                    foreach (var loadLinkExpression in loadLinkExpressions)
                    {
                        loadLinkExpression.AddLookupIds(linkedSource, loadingContext, referenceType);
                    }
                }
            }

            return loadingContext;
        }

        private void LinkNestedLinkedSourcesById(List<Type> referenceTypes)
        {
            foreach (var referenceType in referenceTypes)
            {
                foreach (var linkedSource in _linker.LinkedSourcesToBeBuilt)
                {
                    var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource, referenceType);
                    foreach (var loadLinkExpression in loadLinkExpressions)
                    {
                        loadLinkExpression.LinkNestedLinkedSourceById(linkedSource, _linker, referenceType, _loadLinkProtocol);
                    }
                }
            }
        }

        private void LinkReferences()
        {
            foreach (var linkedSource in _linker.LinkedSourcesToBeBuilt)
            {
                foreach (var loadLinkExpression in _loadLinkProtocol.GetLoadLinkExpressions(linkedSource))
                {
                    loadLinkExpression.LinkReference(linkedSource, _linker);
                }
            }
        }

        private void FilterOutNullValues()
        {
            foreach (var linkedSource in _linker.LinkedSourcesToBeBuilt)
            {
                foreach (var loadLinkExpression in _loadLinkProtocol.GetLoadLinkExpressions(linkedSource))
                {
                    loadLinkExpression.FilterOutNullValues(linkedSource);
                }
            }
        }
    }
}