// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.Debugging;
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

        public LoadLinkerWrapper(Func<IReferenceLoader> createReferenceLoader, IReadOnlyList<IReadOnlyList<Type>> referenceTypeToBeLoadedForEachLoadingLevel, LoadLinkProtocol loadLinkProtocol)
        {
            _createReferenceLoader = createReferenceLoader;
            _referenceTypeToBeLoadedForEachLoadingLevel = referenceTypeToBeLoadedForEachLoadingLevel;
            _loadLinkProtocol = loadLinkProtocol;
        }

        public async Task<TRootLinkedSource> FromModelAsync<TModel>(TModel model, Action<TRootLinkedSource> initRootLinkedSource = null)
        {
            if ((object) model == null)
            {
                return null;
            }

            EnsureValidRootLinkedSourceModelType<TModel>();

            using (var referenceLoader = _createReferenceLoader())
            {
                var loadLinkDetails = GetLoadLinkDetailsIfDebugModeEnabled(nameof(FromModelAsync), new [] { model });
                var loadLinker = CreateLoadLinker(referenceLoader, loadLinkDetails);
                return await loadLinker.FromModelAsync(model, initRootLinkedSource).ConfigureAwait(false);
            }
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> FromModelsAsync<TModel>(IEnumerable<TModel> models, Action<int, TRootLinkedSource> initRootLinkedSources = null)
        {
            if (models.IsNullOrEmpty())
            {
                return new TRootLinkedSource[0];
            }

            EnsureValidRootLinkedSourceModelType<TModel>();

            using (var referenceLoader = _createReferenceLoader())
            {
                var loadLinkDetails = GetLoadLinkDetailsIfDebugModeEnabled(nameof(FromModelsAsync), models);
                var loadLinker = CreateLoadLinker(referenceLoader, loadLinkDetails);
                return await loadLinker.FromModelsAsync(models, initRootLinkedSources).ConfigureAwait(false);
            }
        }

        public async Task<TRootLinkedSource> ByIdAsync<TRootLinkedSourceModelId>(TRootLinkedSourceModelId modelId, Action<TRootLinkedSource> initRootLinkedSource = null)
        {
            if ((object) modelId == null)
            {
                return null;
            }

            using (var referenceLoader = _createReferenceLoader())
            {
                var loadLinkDetails = GetLoadLinkDetailsIfDebugModeEnabled(nameof(ByIdAsync), new [] { modelId });
                var loadLinker = CreateLoadLinker(referenceLoader, loadLinkDetails);
                return await loadLinker.ByIdAsync(modelId, initRootLinkedSource).ConfigureAwait(false);
            }
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> ByIdsAsync<TRootLinkedSourceModelId>(IEnumerable<TRootLinkedSourceModelId> modelIds, Action<int, TRootLinkedSource> initRootLinkedSources = null)
        {
            var nonNullIds = modelIds?
                .Where(id => (object) id != null)
                .ToList();
            if (nonNullIds.IsNullOrEmpty())
            {
                return new TRootLinkedSource[0];
            }

            using (var referenceLoader = _createReferenceLoader())
            {
                var loadLinkDetails = GetLoadLinkDetailsIfDebugModeEnabled(nameof(ByIdsAsync), nonNullIds);
                var loadLinker = CreateLoadLinker(referenceLoader, loadLinkDetails);
                return await loadLinker.ByIdsAsync(nonNullIds, initRootLinkedSources).ConfigureAwait(false);
            }
        }

        private LoadLinkDetails<TRootLinkedSource, TRootLinkedSourceModel> GetLoadLinkDetailsIfDebugModeEnabled<T>(string methodName, IEnumerable<T> values)
        {
            if (!_loadLinkProtocol.IsDebugModeEnabled)
            {
                return null;
            }

            var callDetails = new LoadLinkCallDetails(methodName, values.Cast<object>());
            return new LoadLinkDetails<TRootLinkedSource, TRootLinkedSourceModel>(callDetails, _referenceTypeToBeLoadedForEachLoadingLevel);
        }

        private LoadLinker<TRootLinkedSource, TRootLinkedSourceModel> CreateLoadLinker(IReferenceLoader referenceLoader, LoadLinkDetails<TRootLinkedSource, TRootLinkedSourceModel> loadLinkDetails)
        {
            return new LoadLinker<TRootLinkedSource, TRootLinkedSourceModel>(referenceLoader, _referenceTypeToBeLoadedForEachLoadingLevel, _loadLinkProtocol, loadLinkDetails);
        }

        private static void EnsureValidRootLinkedSourceModelType<TModel>()
        {
            var rootModelType = typeof(TModel);
            var expectedModelType = typeof(TRootLinkedSourceModel);

            if (rootModelType != expectedModelType)
            {
                throw new LinkItException(
                    $"Invalid linked source model. Tried to load link {typeof(TRootLinkedSource).GetFriendlyName()} from model(s) of type {rootModelType.GetFriendlyName()}; expected {expectedModelType.GetFriendlyName()}."
                );
            }
        }
    }
}
