using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LinkedSources;

namespace HeterogeneousDataSources.Protocols
{
    public class LoadLinker<TRootLinkedSource, TExpectedRootLinkedSourceModel> : ILoadLinker<TRootLinkedSource>
        where TRootLinkedSource : class, ILinkedSource<TExpectedRootLinkedSourceModel>, new() 
    {
        private readonly IReferenceLoader _referenceLoader;
        private readonly List<List<Type>> _referenceTypeToBeLoadedForEachLoadingLevel;
        private readonly LoadLinkConfig _config;
        private LoadedReferenceContext _loadedReferenceContext;

        public LoadLinker(IReferenceLoader referenceLoader, List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel, LoadLinkConfig config)
        {
            _referenceLoader = referenceLoader;
            _referenceTypeToBeLoadedForEachLoadingLevel = referenceTypeToBeLoadedForEachLoadingLevel;
            _config = config;
        }

        public TRootLinkedSource FromModel<TRootLinkedSourceModel>(TRootLinkedSourceModel model)
        {
            return FromModels(model)
                .SingleOrDefault();
        }

        public List<TRootLinkedSource> FromModels<TRootLinkedSourceModel>(params TRootLinkedSourceModel[] models){
            using (_referenceLoader){
                //stle: support model that are not class? if not, used null instead of default(T)
                if (models == null) {
                    models = new TRootLinkedSourceModel[] { default(TRootLinkedSourceModel) };
                }

                EnsureValidRootLinkedSourceModelType<TRootLinkedSourceModel>();

                _loadedReferenceContext = new LoadedReferenceContext();

                var linkedSources = models
                    .Cast<TExpectedRootLinkedSourceModel>()
                    .Select(CreateLinkedSource)
                    .ToList();

                LoadLinkRootLinkedSource();
                return linkedSources;
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

        public TRootLinkedSource ById<TRootLinkedSourceModelId>(TRootLinkedSourceModelId modelId)
        {
            return ByIds(modelId)
                .SingleOrDefault();
        }

        public List<TRootLinkedSource> ByIds<TRootLinkedSourceModelId>(params TRootLinkedSourceModelId[] modelIds)
        {
            using (_referenceLoader){
                EnsureModelIdIsNotNull(modelIds);

                var models = LoadRootLinkedSourceModel(modelIds.ToList());
                return FromModels(models.ToArray());
            }
        }

        private void EnsureModelIdIsNotNull<TRootLinkedSourceModelId>(TRootLinkedSourceModelId[] modelIds)
        {
            if (modelIds == null) {
                throw new ArgumentException("The argument modelIds cannot be a null array.");
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
            //stle: do it at creation???? by linkedSource expression
            LinkNestedLinkedSourcesFromModel();

            Load(1);

            LinkReferences();
        }

        private void Load(int startAtLoadingLevel) {
            using (_referenceLoader) {
                for (int loadingLevel = startAtLoadingLevel; loadingLevel < _referenceTypeToBeLoadedForEachLoadingLevel.Count; loadingLevel++) {
                    var referenceTypeToBeLoaded = _referenceTypeToBeLoadedForEachLoadingLevel[loadingLevel];

                    LoadNestingLevel(referenceTypeToBeLoaded);
                    LinkNestedLinkedSourcesById(referenceTypeToBeLoaded);
                    LinkNestedLinkedSourcesFromModel();
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
                    var loadLinkExpressions = _config.GetLoadLinkExpressions(linkedSource, referenceTypeToBeLoaded);
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
                    var loadLinkExpressions = _config.GetLoadLinkExpressions(linkedSource, referenceTypeToBeLoaded);
                    foreach (var loadLinkExpression in loadLinkExpressions) {
                        loadLinkExpression.LinkNestedLinkedSourceById(linkedSource, _loadedReferenceContext, referenceTypeToBeLoaded);
                    }
                }
            }
        }

        private void LinkNestedLinkedSourcesFromModel() {
            while (_loadedReferenceContext.GetLinkedSourceWhereNestedLinkedSourcesFromModelAreNotLinked().Any()) {
                foreach (var linkedSource in _loadedReferenceContext.GetLinkedSourceWhereNestedLinkedSourcesFromModelAreNotLinked()) {
                    var loadLinkExpressions = _config.GetLoadLinkExpressions(linkedSource);
                    foreach (var loadLinkExpression in loadLinkExpressions) {
                        loadLinkExpression.LinkNestedLinkedSourceFromModel(
                            linkedSource,
                            _loadedReferenceContext
                        );
                    }
                    _loadedReferenceContext.AddLinkedSourceWhereNestedLinkedSourcesFromModelAreLinked(linkedSource);
                }
            }
        }

        private void LinkReferences() {
            foreach (var linkedSource in _loadedReferenceContext.LinkedSourcesToBeBuilt) {
                var loadLinkExpressions = _config.GetLoadLinkExpressions(linkedSource);
                foreach (var loadLinkExpression in loadLinkExpressions) {
                    loadLinkExpression.LinkReference(
                        linkedSource,
                        _loadedReferenceContext
                    );
                }
            }
        }

        public TRootLinkedSource CreateLinkedSource(TExpectedRootLinkedSourceModel model) {
            return _loadedReferenceContext
                .CreatePartiallyBuiltLinkedSource<TRootLinkedSource, TExpectedRootLinkedSourceModel>(model);
        }
    }
}