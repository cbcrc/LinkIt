using System;
using System.Collections.Generic;
using System.Linq;

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
                .SingleOrDefault(linkedSource => linkedSource is TLinkedSource);
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
                    LoadNestingLevel<TLinkedSource>(loadingLevel, loadedReferenceContext);
                    LinkNestedLinkedSource<TLinkedSource>(loadingLevel, loadedReferenceContext);
                }
            }
            return loadedReferenceContext;
        }

        private void LoadNestingLevel<TLinkedSource>(int loadingLevel, LoadedReferenceContext loadedReferenceContext)
        {
            var lookupIdContext = GetLookupIdContextForLoadingLevel<TLinkedSource>(loadingLevel, loadedReferenceContext);
            _referenceLoader.LoadReferences(lookupIdContext, loadedReferenceContext);
        }

        private LookupIdContext GetLookupIdContextForLoadingLevel<TLinkedSource>(int loadingLevel, LoadedReferenceContext loadedReferenceContext)
        {
            var lookupIdContext = new LookupIdContext();

            foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt){
                var loadLinkExpressions = _config.GetLoadExpressions<TLinkedSource>(linkedSource, loadingLevel);
                foreach (var loadLinkExpression in loadLinkExpressions){
                    loadLinkExpression.AddLookupIds(linkedSource, lookupIdContext);
                }
            }
            return lookupIdContext;
        }

        private void LinkNestedLinkedSource<TLinkedSource>(int loadingLevel, LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt) {
                var loadLinkExpressions = _config.GetLinkNestedLinkedSourceExpressions<TLinkedSource>(linkedSource, loadingLevel);
                foreach (var loadLinkExpression in loadLinkExpressions) {
                    loadLinkExpression.Link(linkedSource, loadedReferenceContext);
                }
            }
        }

        private void LinkReferences(LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt) {
                var loadLinkExpressions = _config.GetLinkReferenceExpressions(linkedSource);
                foreach (var loadLinkExpression in loadLinkExpressions) {
                    loadLinkExpression.Link(linkedSource, loadedReferenceContext);
                }
            }
        }

        private static TLinkedSource CreateRootLinkedSource<TLinkedSource, TId, TModel>(TId modelId,
            LoadedReferenceContext loadedReferenceContext) where TLinkedSource : ILinkedSource<TModel>, new()
        {
            var models = loadedReferenceContext.GetOptionalReferences<TModel, TId>(new List<TId>{modelId});

            var model = models.SingleOrDefault();
            //stle: constraint TLinkedSource to class and use null
            if (model == null) { return default(TLinkedSource); }

            //what if model is null
            return new TLinkedSource {Model = model};
        }
    }
}
