using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources
{
    public class LoadLinker<TRootLinkedSource, TRootLinkedSourceModel> : ILoadLinker<TRootLinkedSource>
        where TRootLinkedSource : class, ILinkedSource<TRootLinkedSourceModel>, new() 
    {
        private readonly LinkedSourceConfig<TRootLinkedSource, TRootLinkedSourceModel> _linkedSourceConfig;
        //stle: handle dispose in using!
        private readonly IReferenceLoader _referenceLoader;
        private readonly List<List<Type>> _referenceTypeToBeLoadedForEachLoadingLevel;
        private readonly LoadLinkConfig _config;

        public LoadLinker(LinkedSourceConfig<TRootLinkedSource, TRootLinkedSourceModel> linkedSourceConfig, IReferenceLoader referenceLoader, List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel, LoadLinkConfig config)
        {
            _linkedSourceConfig = linkedSourceConfig;
            _referenceLoader = referenceLoader;
            _referenceTypeToBeLoadedForEachLoadingLevel = referenceTypeToBeLoadedForEachLoadingLevel;
            _config = config;
        }

        //stle: fix naming: TRootLinkedSourceModel1
        public TRootLinkedSource FromModel<TRootLinkedSourceModel1>(TRootLinkedSourceModel1 model)
        {
            return FromModel(new List<object> { model })
                .SingleOrDefault();
        }

        public List<TRootLinkedSource> FromModel<TRootLinkedSourceModel1>(List<TRootLinkedSourceModel1> models)
        {
            //stle: beaviour on model null? and id null

            //stle: ensure TRootLinkedSourceModel1=TRootLinkedSourceModel
            var loadedReferenceContext = new LoadedReferenceContext();

            var linkedSources = models
                .Cast<TRootLinkedSourceModel>()
                .Select(model => CreateLinkedSource(model, loadedReferenceContext))
                .ToList();

            LoadLinkRootLinkedSource(loadedReferenceContext);
            return linkedSources;
        }

        public TRootLinkedSource ById<TRootLinkedSourceModelId>(TRootLinkedSourceModelId modelId)
        {
            if (modelId == null) { throw new ArgumentNullException("modelId"); }

            var lookupIdContext = new LookupIdContext();
            lookupIdContext.AddSingle<TRootLinkedSourceModel, TRootLinkedSourceModelId>(modelId);

            //stle: where to start context?
            var loadedReferenceContext = new LoadedReferenceContext();
            _referenceLoader.LoadReferences(lookupIdContext, loadedReferenceContext);
            var model = loadedReferenceContext
                .GetOptionalReference<TRootLinkedSourceModel, TRootLinkedSourceModelId>(modelId);

            return FromModel(model);
        }

        private void LoadLinkRootLinkedSource(LoadedReferenceContext loadedReferenceContext) {
            //stle: do it at creation???? by linkedSource expression
            LinkSubLinkedSources(loadedReferenceContext);

            Load(loadedReferenceContext, 0);

            LinkReferences(loadedReferenceContext);
        }

        private void Load(LoadedReferenceContext loadedReferenceContext, int startAtLoadingLevel) {
            using (_referenceLoader) {
                for (int loadingLevel = startAtLoadingLevel; loadingLevel < _referenceTypeToBeLoadedForEachLoadingLevel.Count; loadingLevel++) {
                    var referenceTypeToBeLoaded = _referenceTypeToBeLoadedForEachLoadingLevel[loadingLevel];

                    LoadNestingLevel(loadedReferenceContext, referenceTypeToBeLoaded);
                    LinkNestedLinkedSources(loadedReferenceContext, referenceTypeToBeLoaded);
                    LinkSubLinkedSources(loadedReferenceContext);
                }
            }
        }

        private void LoadNestingLevel(LoadedReferenceContext loadedReferenceContext, List<Type> referenceTypeToBeLoaded) {
            var lookupIdContext = GetLookupIdContextForLoadingLevel(loadedReferenceContext, referenceTypeToBeLoaded);
            _referenceLoader.LoadReferences(lookupIdContext, loadedReferenceContext);
        }

        private LookupIdContext GetLookupIdContextForLoadingLevel(LoadedReferenceContext loadedReferenceContext, List<Type> referenceTypesToBeLoaded) {
            var lookupIdContext = new LookupIdContext();

            foreach (var referenceTypeToBeLoaded in referenceTypesToBeLoaded) {
                foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt) {
                    var loadLinkExpressions = _config.GetLoadLinkExpressions(linkedSource, referenceTypeToBeLoaded);
                    foreach (var loadLinkExpression in loadLinkExpressions) {
                        loadLinkExpression.AddLookupIds(linkedSource, lookupIdContext, referenceTypeToBeLoaded);
                    }
                }
            }
            return lookupIdContext;
        }

        private void LinkNestedLinkedSources(LoadedReferenceContext loadedReferenceContext, List<Type> referenceTypesToBeLoaded) {
            foreach (var referenceTypeToBeLoaded in referenceTypesToBeLoaded) {
                foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt) {
                    var loadLinkExpressions = _config.GetLoadLinkExpressions(linkedSource, referenceTypeToBeLoaded);
                    foreach (var loadLinkExpression in loadLinkExpressions) {
                        loadLinkExpression.LinkNestedLinkedSource(linkedSource, loadedReferenceContext, referenceTypeToBeLoaded);
                    }
                }
            }
        }

        private void LinkSubLinkedSources(LoadedReferenceContext loadedReferenceContext) {
            while (loadedReferenceContext.GetLinkedSourceWithSubLinkedSourceToLink().Any()) {
                foreach (var linkedSource in loadedReferenceContext.GetLinkedSourceWithSubLinkedSourceToLink()) {
                    var loadLinkExpressions = _config.GetLoadExpressions(linkedSource);
                    foreach (var loadLinkExpression in loadLinkExpressions) {
                        loadLinkExpression.LinkSubLinkedSource(
                            linkedSource,
                            loadedReferenceContext
                        );
                    }
                    loadedReferenceContext.AddLinkedSourceWhereSubLinkedSourceAreLinked(linkedSource);
                }
            }
        }

        private void LinkReferences(LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt) {
                var loadLinkExpressions = _config.GetLoadExpressions(linkedSource);
                foreach (var loadLinkExpression in loadLinkExpressions) {
                    loadLinkExpression.LinkReference(
                        linkedSource,
                        loadedReferenceContext
                    );
                }
            }
        }

        ////Is LoadLinkExpressionUtil still required?
        public TRootLinkedSource CreateLinkedSource(TRootLinkedSourceModel model, LoadedReferenceContext loadedReferenceContext) {
            return LoadLinkExpressionUtil.CreateLinkedSource<TRootLinkedSource, TRootLinkedSourceModel>(
                model,
                loadedReferenceContext
            );
        }
    }
}