#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    internal class LoadLinkerProxy<TRootLinkedSource, TRootLinkedSourceModel> : ILoadLinker<TRootLinkedSource>
        where TRootLinkedSource : class, ILinkedSource<TRootLinkedSourceModel>, new()
    {
        private readonly Func<IReferenceLoader> _createReferenceLoader;
        private readonly List<List<Type>> _referenceTypeToBeLoadedForEachLoadingLevel;
        private readonly LoadLinkProtocol _loadLinkProtocol;

        public LoadLinkerProxy(Func<IReferenceLoader> createReferenceLoader, List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel, LoadLinkProtocol loadLinkProtocol)
        {
            _createReferenceLoader = createReferenceLoader;
            _referenceTypeToBeLoadedForEachLoadingLevel = referenceTypeToBeLoadedForEachLoadingLevel;
            _loadLinkProtocol = loadLinkProtocol;
        }

        public async Task<TRootLinkedSource> FromModelAsync<TModel>(TModel model, Action<TRootLinkedSource> initRootLinkedSource = null)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            EnsureValidRootLinkedSourceModelType<TModel>();

            using (var referenceLoader = _createReferenceLoader())
            {
                var loadLinker = CreateLoadLinker(referenceLoader);
                return await loadLinker.FromModelAsync(model, initRootLinkedSource);
            }
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> FromModelsAsync<TModel>(IEnumerable<TModel> models, Action<int, TRootLinkedSource> initRootLinkedSources = null)
        {
            if (models == null) throw new ArgumentNullException(nameof(models));
            EnsureValidRootLinkedSourceModelType<TModel>();

            using (var referenceLoader = _createReferenceLoader())
            {
                var loadLinker = CreateLoadLinker(referenceLoader);
                return await loadLinker.FromModelsAsync(models, initRootLinkedSources);
            }
        }

        public async Task<TRootLinkedSource> ByIdAsync<TRootLinkedSourceModelId>(TRootLinkedSourceModelId modelId, Action<TRootLinkedSource> initRootLinkedSource = null)
        {
            if (modelId == null) throw new ArgumentNullException(nameof(modelId));

            using (var referenceLoader = _createReferenceLoader())
            {
                var loadLinker = CreateLoadLinker(referenceLoader);
                return await loadLinker.ByIdAsync(modelId, initRootLinkedSource);
            }
        }

        public async Task<IReadOnlyList<TRootLinkedSource>> ByIdsAsync<TRootLinkedSourceModelId>(IEnumerable<TRootLinkedSourceModelId> modelIds, Action<int, TRootLinkedSource> initRootLinkedSources = null)
        {
            if (modelIds == null) throw new ArgumentNullException(nameof(modelIds));

            using (var referenceLoader = _createReferenceLoader())
            {
                var loadLinker = CreateLoadLinker(referenceLoader);
                return await loadLinker.ByIdsAsync(modelIds, initRootLinkedSources);
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
                throw new ArgumentException(
                    $"Invalid root linked source model type. Expected {expectedModelType} but was {rootModelType}.",
                    nameof(TRootLinkedSourceModel)
                );
            }
        }
    }
}