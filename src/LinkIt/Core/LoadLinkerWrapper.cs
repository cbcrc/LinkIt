// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.PublicApi;
using LinkIt.ReadableExpressions.Extensions;
using LinkIt.Shared;

namespace LinkIt.Core
{
    internal class LoadLinkerWrapper<TRootLinkedSource, TRootLinkedSourceModel> : ILoadLinker<TRootLinkedSource>
        where TRootLinkedSource : class, ILinkedSource<TRootLinkedSourceModel>, new()
    {
        private readonly Func<IReferenceLoader> _createReferenceLoader;
        private readonly List<List<Type>> _referenceTypeToBeLoadedForEachLoadingLevel;
        private readonly LoadLinkProtocol _loadLinkProtocol;

        public LoadLinkerWrapper(Func<IReferenceLoader> createReferenceLoader, List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel, LoadLinkProtocol loadLinkProtocol)
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
                var loadLinker = CreateLoadLinker(referenceLoader);
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
                var loadLinker = CreateLoadLinker(referenceLoader);
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
                var loadLinker = CreateLoadLinker(referenceLoader);
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
                var loadLinker = CreateLoadLinker(referenceLoader);
                return await loadLinker.ByIdsAsync(nonNullIds, initRootLinkedSources).ConfigureAwait(false);
            }
        }

        private LoadLinker<TRootLinkedSource, TRootLinkedSourceModel> CreateLoadLinker(IReferenceLoader referenceLoader)
        {
            return new LoadLinker<TRootLinkedSource, TRootLinkedSourceModel>(referenceLoader, _referenceTypeToBeLoadedForEachLoadingLevel, _loadLinkProtocol);
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