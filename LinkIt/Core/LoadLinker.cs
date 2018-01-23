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
    //See ILoadLinker<TRootLinkedSource>
    public class LoadLinker<TRootLinkedSource, TExpectedRootLinkedSourceModel> : ILoadLinker<TRootLinkedSource>
        where TRootLinkedSource : class, ILinkedSource<TExpectedRootLinkedSourceModel>, new()
    {
        private readonly LoadLinkProtocol _loadLinkProtocol;
        private readonly IReferenceLoader _referenceLoader;
        private readonly List<List<Type>> _referenceTypeToBeLoadedForEachLoadingLevel;
        private LoadedReferenceContext _loadedReferenceContext;

        public LoadLinker(IReferenceLoader referenceLoader, List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel, LoadLinkProtocol loadLinkProtocol)
        {
            _referenceLoader = referenceLoader;
            _referenceTypeToBeLoadedForEachLoadingLevel = referenceTypeToBeLoadedForEachLoadingLevel;
            _loadLinkProtocol = loadLinkProtocol;
        }

        public async Task<TRootLinkedSource> FromModelAsync<TRootLinkedSourceModel>(
            TRootLinkedSourceModel model,
            Action<TRootLinkedSource> initRootLinkedSource)
        {
            var linkedSources = await FromModelsAsync(
                new List<TRootLinkedSourceModel> { model },
                ToInitRootLinkedSources(initRootLinkedSource)
            );
            return linkedSources.SingleOrDefault();
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> FromModelsAsync<TRootLinkedSourceModel>(
            IEnumerable<TRootLinkedSourceModel> models,
            Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            if (models == null) throw new ArgumentNullException(nameof(models));

            using (_referenceLoader)
            {
                EnsureValidRootLinkedSourceModelType<TRootLinkedSourceModel>();

                _loadedReferenceContext = new LoadedReferenceContext();

                var linkedSources = models
                    .Cast<TExpectedRootLinkedSourceModel>()
                    .Select((model, index) => CreateLinkedSource(model, index, initRootLinkedSources))
                    .ToList();

                await LoadLinkRootLinkedSource();

                return linkedSources
                    .Where(linkedSource => linkedSource != null)
                    .ToList();
            }
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
            if (modelIds == null) throw new ArgumentNullException(nameof(modelIds));

            using (_referenceLoader)
            {
                var models = await LoadRootLinkedSourceModelAsync(modelIds.ToList());
                return await FromModelsAsync(
                    models,
                    initRootLinkedSources
                );
            }
        }

        private static void EnsureValidRootLinkedSourceModelType<TRootLinkedSourceModel>()
        {
            var rootLinkedSourceModelType1 = typeof(TRootLinkedSourceModel);
            var rootLinkedSourceModelType = typeof(TExpectedRootLinkedSourceModel);

            if (rootLinkedSourceModelType1 != rootLinkedSourceModelType)
                throw new ArgumentException(
                    $"Invalid root linked source model type. Expected {rootLinkedSourceModelType} but was {rootLinkedSourceModelType1}.",
                    "TRootLinkedSourceModel"
                );
        }

        private TRootLinkedSource CreateLinkedSource(TExpectedRootLinkedSourceModel model, int index, Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            return _loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource(
                    model,
                    _loadLinkProtocol,
                    CreateInitChildLinkedSourceAction(
                        initRootLinkedSources,
                        index
                    )
                );
        }

        private static Action<TRootLinkedSource> CreateInitChildLinkedSourceAction(Action<int, TRootLinkedSource> initRootLinkedSources, int index)
        {
            if (initRootLinkedSources == null) return null;

            return childLinkedSource => initRootLinkedSources(index, childLinkedSource);
        }

        private static Action<int, TRootLinkedSource> ToInitRootLinkedSources(Action<TRootLinkedSource> initRootLinkedSource)
        {
            if (initRootLinkedSource == null) return null;

            return (referenceIndex, rootLinkedSource) => initRootLinkedSource(rootLinkedSource);
        }

        private async Task<IEnumerable<TExpectedRootLinkedSourceModel>> LoadRootLinkedSourceModelAsync<TRootLinkedSourceModelId>(IEnumerable<TRootLinkedSourceModelId> modelIds)
        {
            var lookupIdContext = new LookupIdContext();
            lookupIdContext.AddMulti<TExpectedRootLinkedSourceModel, TRootLinkedSourceModelId>(modelIds);

            var loadedRootLinkedSourceModel = new LoadedReferenceContext();
            await _referenceLoader.LoadReferencesAsync(lookupIdContext, loadedRootLinkedSourceModel);

            return loadedRootLinkedSourceModel
                .GetOptionalReferences<TExpectedRootLinkedSourceModel, TRootLinkedSourceModelId>(modelIds);
        }

        private async Task LoadLinkRootLinkedSource()
        {
            await LoadAsync(1);

            LinkReferences();

            FilterOutNullValues();
        }

        private async Task LoadAsync(int startAtLoadingLevel)
        {
            using (_referenceLoader)
            {
                for (var loadingLevel = startAtLoadingLevel; loadingLevel < _referenceTypeToBeLoadedForEachLoadingLevel.Count; loadingLevel++)
                {
                    var referenceTypeToBeLoaded = _referenceTypeToBeLoadedForEachLoadingLevel[loadingLevel];

                    await LoadNestingLevelAsync(referenceTypeToBeLoaded);
                    LinkNestedLinkedSourcesById(referenceTypeToBeLoaded);
                }
            }
        }

        private async Task LoadNestingLevelAsync(List<Type> referenceTypeToBeLoaded)
        {
            var lookupIdContext = GetLookupIdContextForLoadingLevel(referenceTypeToBeLoaded);
            await _referenceLoader.LoadReferencesAsync(lookupIdContext, _loadedReferenceContext);
        }

        private LookupIdContext GetLookupIdContextForLoadingLevel(List<Type> referenceTypesToBeLoaded)
        {
            var lookupIdContext = new LookupIdContext();

            foreach (var referenceTypeToBeLoaded in referenceTypesToBeLoaded)
            foreach (var linkedSource in _loadedReferenceContext.GetLinkedSourcesToBeBuilt())
            {
                var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource, referenceTypeToBeLoaded);
                foreach (var loadLinkExpression in loadLinkExpressions) loadLinkExpression.AddLookupIds(linkedSource, lookupIdContext, referenceTypeToBeLoaded);
            }

            return lookupIdContext;
        }

        private void LinkNestedLinkedSourcesById(List<Type> referenceTypesToBeLoaded)
        {
            foreach (var referenceTypeToBeLoaded in referenceTypesToBeLoaded)
            foreach (var linkedSource in _loadedReferenceContext.GetLinkedSourcesToBeBuilt())
            {
                var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource, referenceTypeToBeLoaded);
                foreach (var loadLinkExpression in loadLinkExpressions) loadLinkExpression.LinkNestedLinkedSourceById(linkedSource, _loadedReferenceContext, referenceTypeToBeLoaded, _loadLinkProtocol);
            }
        }

        private void LinkReferences()
        {
            foreach (var linkedSource in _loadedReferenceContext.GetLinkedSourcesToBeBuilt())
            {
                var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource);
                foreach (var loadLinkExpression in loadLinkExpressions)
                    loadLinkExpression.LinkReference(
                        linkedSource,
                        _loadedReferenceContext
                    );
            }
        }

        private void FilterOutNullValues()
        {
            foreach (var linkedSource in _loadedReferenceContext.GetLinkedSourcesToBeBuilt())
            {
                var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource);
                foreach (var loadLinkExpression in loadLinkExpressions) loadLinkExpression.FilterOutNullValues(linkedSource);
            }
        }
    }
}