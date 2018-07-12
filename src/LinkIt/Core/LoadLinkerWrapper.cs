// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.Diagnostics;
using LinkIt.PublicApi;
using LinkIt.ReadableExpressions.Extensions;
using LinkIt.Shared;

namespace LinkIt.Core
{
    internal class LoadLinkerWrapper<TRootLinkedSource, TRootLinkedSourceModel> : ILoadLinker<TRootLinkedSource>
        where TRootLinkedSource : class, ILinkedSource<TRootLinkedSourceModel>, new()
    {
        private readonly Func<IReferenceLoader> _createReferenceLoader;
        private readonly IReadOnlyList<IReadOnlyList<Type>> _referenceTypeToBeLoadedForEachLoadingLevel;
        private readonly LoadLinkProtocol _loadLinkProtocol;
        private bool _isDebugModeEnabled;

        private Action<ILoadLinkDetails> _onLoadLinkCompleted;

        public LoadLinkerWrapper(Func<IReferenceLoader> createReferenceLoader, IReadOnlyList<IReadOnlyList<Type>> referenceTypeToBeLoadedForEachLoadingLevel, LoadLinkProtocol loadLinkProtocol)
        {
            _createReferenceLoader = createReferenceLoader;
            _referenceTypeToBeLoadedForEachLoadingLevel = referenceTypeToBeLoadedForEachLoadingLevel;
            _loadLinkProtocol = loadLinkProtocol;
        }

        public ILoadLinker<TRootLinkedSource> EnableDebugMode(Action<ILoadLinkDetails> onLoadLinkCompleted = null)
        {
            _isDebugModeEnabled = true;
            _onLoadLinkCompleted = onLoadLinkCompleted;

            return this;
        }

        public async Task<TRootLinkedSource> FromModelAsync<TModel>(TModel model, Action<TRootLinkedSource> initRootLinkedSource = null)
        {
            if ((object) model == null)
            {
                return null;
            }

            if (!(model is TRootLinkedSourceModel))
            {
                throw InvalidModelType(model.GetType());
            }

            var nonNullModels = new [] { (TRootLinkedSourceModel) (object) model };
            var linkedSources = await LoadLinkFromModelsAsync(nameof(FromModelAsync), nonNullModels, InitRootLinkedSourceWithIndex(initRootLinkedSource)).ConfigureAwait(false);
            return linkedSources.SingleOrDefault();
        }

        private static Action<int, TRootLinkedSource> InitRootLinkedSourceWithIndex(Action<TRootLinkedSource> initRootLinkedSource)
        {
            return (_, linkedSource) => initRootLinkedSource?.Invoke(linkedSource);
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> FromModelsAsync<TModel>(IEnumerable<TModel> models, Action<int, TRootLinkedSource> initRootLinkedSources = null)
        {
            if (!typeof(TRootLinkedSourceModel).IsAssignableFrom(typeof(TModel)))
            {
                throw InvalidModelType(typeof(TModel));
            }

            if (models is null)
            {
                return new TRootLinkedSource[0];
            }

            var nonNullModels = models
                .WhereNotNull()
                .Cast<TRootLinkedSourceModel>()
                .ToList();

            if (nonNullModels.Count == 0)
            {
                return new TRootLinkedSource[0];
            }

            return await LoadLinkFromModelsAsync(nameof(FromModelsAsync), nonNullModels, initRootLinkedSources).ConfigureAwait(false);
        }

        private async Task<IReadOnlyList<TRootLinkedSource>> LoadLinkFromModelsAsync(string callingMethod, IEnumerable<TRootLinkedSourceModel> nonNullModels, Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            var loadLinkDetails = GetLoadLinkDetailsIfDebugModeEnabled(callingMethod, nonNullModels);
            loadLinkDetails?.CurrentStep.AddReferenceValues(nonNullModels);

            IReadOnlyList<TRootLinkedSource> linkedSources;
            using (var loadLinker = CreateLoadLinker(loadLinkDetails))
            {
                linkedSources = await loadLinker.FromModelsAsync(nonNullModels, initRootLinkedSources).ConfigureAwait(false);
            }

            OnLoadLinkCompleted(loadLinkDetails);
            return linkedSources;
        }

        public async Task<TRootLinkedSource> ByIdAsync<TRootLinkedSourceModelId>(TRootLinkedSourceModelId modelId, Action<TRootLinkedSource> initRootLinkedSource = null)
        {
            if ((object) modelId == null)
            {
                return null;
            }

            var nonNullIds = new [] { modelId };
            var linkedSources = await LoadLinkFromIdsAsync(nameof(ByIdsAsync), nonNullIds, InitRootLinkedSourceWithIndex(initRootLinkedSource)).ConfigureAwait(false);
            return linkedSources.SingleOrDefault();
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> ByIdsAsync<TRootLinkedSourceModelId>(IEnumerable<TRootLinkedSourceModelId> modelIds, Action<int, TRootLinkedSource> initRootLinkedSources = null)
        {
            var nonNullIds = modelIds?
                .WhereNotNull()
                .ToList();
            if (nonNullIds.IsNullOrEmpty())
            {
                return new TRootLinkedSource[0];
            }

            return await LoadLinkFromIdsAsync(nameof(ByIdsAsync), nonNullIds, initRootLinkedSources).ConfigureAwait(false);
        }

        private async Task<IReadOnlyList<TRootLinkedSource>> LoadLinkFromIdsAsync<TRootLinkedSourceModelId>(string callingMethod, IEnumerable<TRootLinkedSourceModelId> nonNullModelIds, Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            var loadLinkDetails = GetLoadLinkDetailsIfDebugModeEnabled(callingMethod, nonNullModelIds);

            IReadOnlyList<TRootLinkedSource> linkedSources;
            using (var loadLinker = CreateLoadLinker(loadLinkDetails))
            {
                linkedSources = await loadLinker.ByIdsAsync(nonNullModelIds, initRootLinkedSources).ConfigureAwait(false);
            }

            OnLoadLinkCompleted(loadLinkDetails);
            return linkedSources;
        }

        private static LinkItException InvalidModelType(Type modelType)
        {
            return new LinkItException(
                $"Invalid linked source model. Tried to load link {typeof(TRootLinkedSource).GetFriendlyName()} from model(s) of type {modelType.GetFriendlyName()}; expected {typeof(TRootLinkedSourceModel).GetFriendlyName()}."
            );
        }

        private LoadLinkDetails<TRootLinkedSource, TRootLinkedSourceModel> GetLoadLinkDetailsIfDebugModeEnabled(string methodName, IEnumerable values)
        {
            if (!_isDebugModeEnabled)
            {
                return null;
            }

            var callDetails = new LoadLinkCallDetails(methodName, values);
            return new LoadLinkDetails<TRootLinkedSource, TRootLinkedSourceModel>(callDetails, _referenceTypeToBeLoadedForEachLoadingLevel);
        }

        private LoadLinker<TRootLinkedSource, TRootLinkedSourceModel> CreateLoadLinker(LoadLinkDetails<TRootLinkedSource, TRootLinkedSourceModel> loadLinkDetails)
        {
            return new LoadLinker<TRootLinkedSource, TRootLinkedSourceModel>(_createReferenceLoader, _referenceTypeToBeLoadedForEachLoadingLevel, _loadLinkProtocol, loadLinkDetails);
        }

        private void OnLoadLinkCompleted(LoadLinkDetails<TRootLinkedSource, TRootLinkedSourceModel> loadLinkDetails)
        {
            if (!_isDebugModeEnabled || loadLinkDetails is null)
            {
                return;
            }

            loadLinkDetails.LoadLinkEnd();
            _onLoadLinkCompleted?.Invoke(loadLinkDetails);
        }
    }
}
