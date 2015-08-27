using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkProtocol {
        private readonly IReferenceLoader _referenceLoader;
        private readonly LoadLinkConfig _config;

        public LoadLinkProtocol(IReferenceLoader referenceLoader, LoadLinkConfig config)
        {
            _referenceLoader = referenceLoader;
            _config = config;
        }

        public TLinkedSource LoadLink<TLinkedSource,TId, TModel>(TId modelId)
            where TLinkedSource : ILinkedSource<TModel>, new() 
        {
            var loadedReferenceContext = Load<TLinkedSource, TId, TModel>(modelId);
            
            LinkReferences(loadedReferenceContext);

            return (TLinkedSource)loadedReferenceContext.LinkedSourcesToBeBuilt
                .Single(linkedSource => linkedSource is TLinkedSource);
        }

        private LoadedReferenceContext Load<TLinkedSource,TId, TModel>(TId modelId)
            where TLinkedSource :ILinkedSource<TModel>, new() 
        {
            var loadedReferenceContext = new LoadedReferenceContext();
            
            var lookupIdContextLevel0 = new LookupIdContext();
            lookupIdContextLevel0.Add<TModel, TId>(new List<TId> { modelId });

            using (_referenceLoader)
            {
                _referenceLoader.LoadReferences(lookupIdContextLevel0, loadedReferenceContext);

                var rootLinkedSource = CreateRootLinkedSource<TLinkedSource, TId, TModel>(modelId, loadedReferenceContext);
                loadedReferenceContext.AddLinkedSourceToBeBuilt(rootLinkedSource);

                var numberOfLoadingLevel = _config.GetNumberOfLoadingLevel<TLinkedSource>();
                //stle: 1 to skip root for now
                for (int loadingLevel = 1; loadingLevel < numberOfLoadingLevel; loadingLevel++)
                {
                    var referenceTypesToBeLoaded = _config.GetReferenceTypeForLoadingLevel<TLinkedSource>(loadingLevel);
                    LoadNextNestingLevel(loadedReferenceContext, referenceTypesToBeLoaded);
                }
            }
            return loadedReferenceContext;
        }

        private void LoadNextNestingLevel(LoadedReferenceContext loadedReferenceContext, List<Type> referenceTypesToBeLoaded)
        {
            var lookupIdContext = new LookupIdContext();

            var loadLinkExpressions = _config.AllLoadLinkExpressions
                .Where(loadLinkExpression => referenceTypesToBeLoaded.Contains(loadLinkExpression.ReferenceType))
                .ToList();

            foreach (var loadLinkExpression in loadLinkExpressions)
            {
                loadLinkExpression.AddLookupIds(
                    loadedReferenceContext.LinkedSourcesToBeBuilt, 
                    lookupIdContext
                );
            }
            _referenceLoader.LoadReferences(lookupIdContext, loadedReferenceContext);
            LinkNestedLinkedSource(loadedReferenceContext);
        }

        private void LinkNestedLinkedSource(LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkExpression in _config.NestedLinkedSourceLoadLinkExpressions) {
                linkExpression.Link(loadedReferenceContext);
            }
        }


        private static TLinkedSource CreateRootLinkedSource<TLinkedSource, TId, TModel>(TId modelId,
            LoadedReferenceContext loadedReferenceContext) where TLinkedSource : ILinkedSource<TModel>, new()
        {
            var model = loadedReferenceContext.GetOptionalReference<TModel, TId>(modelId);

            //what if model is null

            var rootLinkedSource = new TLinkedSource();
            rootLinkedSource.Model = model;
            return rootLinkedSource;
        }

        private void LinkReferences(LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkExpression in _config.ReferenceLoadLinkExpressions) {
                linkExpression.Link(loadedReferenceContext);
            }
        }
    }
}
