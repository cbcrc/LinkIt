#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Core
{
    //See ILoadLinker<TRootLinkedSource>
    public class LoadLinker<TRootLinkedSource, TExpectedRootLinkedSourceModel> : ILoadLinker<TRootLinkedSource>
        where TRootLinkedSource : class, ILinkedSource<TExpectedRootLinkedSourceModel>, new() 
    {
        private readonly IReferenceLoader _referenceLoader;
        private readonly List<List<Type>> _referenceTypeToBeLoadedForEachLoadingLevel;
        private readonly LoadLinkProtocol _loadLinkProtocol;
        private LoadedReferenceContext _loadedReferenceContext;

        public LoadLinker(IReferenceLoader referenceLoader, List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel, LoadLinkProtocol loadLinkProtocol)
        {
            _referenceLoader = referenceLoader;
            _referenceTypeToBeLoadedForEachLoadingLevel = referenceTypeToBeLoadedForEachLoadingLevel;
            _loadLinkProtocol = loadLinkProtocol;
        }

        public TRootLinkedSource FromModel<TRootLinkedSourceModel>(
            TRootLinkedSourceModel model,
            Action<TRootLinkedSource> initRootLinkedSource)
        {
            return FromModels(
                new List<TRootLinkedSourceModel>{model},
                ToInitRootLinkedSources(initRootLinkedSource)
            )
            .SingleOrDefault();
        }

        public List<TRootLinkedSource> FromModels<TRootLinkedSourceModel>(
            List<TRootLinkedSourceModel> models,
            Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            if (models == null) { throw new ArgumentNullException("models"); }

            using (_referenceLoader){
                EnsureValidRootLinkedSourceModelType<TRootLinkedSourceModel>();

                _loadedReferenceContext = new LoadedReferenceContext();

                var linkedSources = models
                    .Cast<TExpectedRootLinkedSourceModel>()
                    .Select((model,index)=>CreateLinkedSource(model, index, initRootLinkedSources))
                    .ToList();

                LoadLinkRootLinkedSource();

                return linkedSources
                    .Where(linkedSource=>linkedSource!=null)
                    .ToList();
            }
        }

        private void EnsureValidRootLinkedSourceModelType<TRootLinkedSourceModel>()
        {
            var rootLinkedSourceModelType1 = typeof(TRootLinkedSourceModel);
            var rootLinkedSourceModelType = typeof(TExpectedRootLinkedSourceModel);

            if (rootLinkedSourceModelType1 != rootLinkedSourceModelType){
                throw new ArgumentException(
                    string.Format(
                        "Invalid root linked source model type. Expected {0} but was {1}.",
                        rootLinkedSourceModelType,
                        rootLinkedSourceModelType1
                    ),
                    "TRootLinkedSourceModel"
                );
            }
        }

        private TRootLinkedSource CreateLinkedSource(TExpectedRootLinkedSourceModel model, int index, Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            return _loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource<TRootLinkedSource, TExpectedRootLinkedSourceModel>(
                    model, 
                    _loadLinkProtocol, 
                    CreateInitChildLinkedSourceAction(
                        initRootLinkedSources,
                        index
                    )
                );
        }

        private Action<TRootLinkedSource> CreateInitChildLinkedSourceAction(Action<int, TRootLinkedSource> initRootLinkedSources, int index) {
            if (initRootLinkedSources == null) { return null; }

            return childLinkedSource => initRootLinkedSources(index, childLinkedSource);
        }


        public TRootLinkedSource ById<TRootLinkedSourceModelId>(
            TRootLinkedSourceModelId modelId,
            Action<TRootLinkedSource> initRootLinkedSource)
        {
            return ByIds(
                new List<TRootLinkedSourceModelId>{modelId},
                ToInitRootLinkedSources(initRootLinkedSource)
            )
            .SingleOrDefault();
        }

        private static Action<int, TRootLinkedSource> ToInitRootLinkedSources(Action<TRootLinkedSource> initRootLinkedSource)
        {
            if (initRootLinkedSource == null) { return null; }

            return (referenceIndex,rootLinkedSource)=>initRootLinkedSource(rootLinkedSource);
        }

        public List<TRootLinkedSource> ByIds<TRootLinkedSourceModelId>(
            List<TRootLinkedSourceModelId> modelIds,
            Action<int, TRootLinkedSource> initRootLinkedSources)
        {
            if (modelIds == null) { throw new ArgumentNullException("modelIds"); }

            using (_referenceLoader){
                var models = LoadRootLinkedSourceModel(modelIds.ToList());
                return FromModels(
                    models,
                    initRootLinkedSources
                );
            }
        }

        private List<TExpectedRootLinkedSourceModel> LoadRootLinkedSourceModel<TRootLinkedSourceModelId>(List<TRootLinkedSourceModelId> modelIds){
            var lookupIdContext = new LookupIdContext();
            lookupIdContext.AddMulti<TExpectedRootLinkedSourceModel, TRootLinkedSourceModelId>(modelIds);

            var loadedRootLinkedSourceModel = new LoadedReferenceContext();
            _referenceLoader.LoadReferences(lookupIdContext, loadedRootLinkedSourceModel);

            return loadedRootLinkedSourceModel
                .GetOptionalReferences<TExpectedRootLinkedSourceModel, TRootLinkedSourceModelId>(modelIds);
        }

        private void LoadLinkRootLinkedSource() {
            Load(1);

            LinkReferences();

            FilterOutNullValues();
        }

        private void Load(int startAtLoadingLevel) {
            using (_referenceLoader) {
                for (int loadingLevel = startAtLoadingLevel; loadingLevel < _referenceTypeToBeLoadedForEachLoadingLevel.Count; loadingLevel++) {
                    var referenceTypeToBeLoaded = _referenceTypeToBeLoadedForEachLoadingLevel[loadingLevel];

                    LoadNestingLevel(referenceTypeToBeLoaded);
                    LinkNestedLinkedSourcesById(referenceTypeToBeLoaded);
                }
            }
        }

        private void LoadNestingLevel(List<Type> referenceTypeToBeLoaded) {
            var lookupIdContext = GetLookupIdContextForLoadingLevel(referenceTypeToBeLoaded);
            _referenceLoader.LoadReferences(lookupIdContext, _loadedReferenceContext);
        }

        private LookupIdContext GetLookupIdContextForLoadingLevel(List<Type> referenceTypesToBeLoaded) {
            var lookupIdContext = new LookupIdContext();

            foreach (var referenceTypeToBeLoaded in referenceTypesToBeLoaded) {
                foreach (var linkedSource in _loadedReferenceContext.LinkedSourcesToBeBuilt) {
                    var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource, referenceTypeToBeLoaded);
                    foreach (var loadLinkExpression in loadLinkExpressions) {
                        loadLinkExpression.AddLookupIds(linkedSource, lookupIdContext, referenceTypeToBeLoaded);
                    }
                }
            }
            return lookupIdContext;
        }

        private void LinkNestedLinkedSourcesById(List<Type> referenceTypesToBeLoaded) {
            foreach (var referenceTypeToBeLoaded in referenceTypesToBeLoaded) {
                foreach (var linkedSource in _loadedReferenceContext.LinkedSourcesToBeBuilt) {
                    var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource, referenceTypeToBeLoaded);
                    foreach (var loadLinkExpression in loadLinkExpressions) {
                        loadLinkExpression.LinkNestedLinkedSourceById(linkedSource, _loadedReferenceContext, referenceTypeToBeLoaded, _loadLinkProtocol);
                    }
                }
            }
        }

        private void LinkReferences() {
            foreach (var linkedSource in _loadedReferenceContext.LinkedSourcesToBeBuilt) {
                var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource);
                foreach (var loadLinkExpression in loadLinkExpressions) {
                    loadLinkExpression.LinkReference(
                        linkedSource,
                        _loadedReferenceContext
                    );
                }
            }
        }

        private void FilterOutNullValues() {
            foreach (var linkedSource in _loadedReferenceContext.LinkedSourcesToBeBuilt) {
                var loadLinkExpressions = _loadLinkProtocol.GetLoadLinkExpressions(linkedSource);
                foreach (var loadLinkExpression in loadLinkExpressions) {
                    loadLinkExpression.FilterOutNullValues(linkedSource);
                }
            }
        }

    }
}