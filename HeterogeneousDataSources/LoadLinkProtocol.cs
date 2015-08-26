using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkProtocol {
        private readonly IReferenceLoader _referenceLoader;
        private readonly List<ILoadLinkExpression> _loadLinkExpressions;

        public LoadLinkProtocol(IReferenceLoader referenceLoader, List<ILoadLinkExpression> loadLinkExpressions)
        {
            _referenceLoader = referenceLoader;
            _loadLinkExpressions = loadLinkExpressions;
        }

        public TLinkedSource LoadLink<TLinkedSource,TId, TModel>(TId modelId)
            where TLinkedSource : ILinkedSource<TModel>, new() 
        {
            var loadedReferenceContext = Load<TLinkedSource, TId, TModel>(modelId);

            var rootLinkedSource = GetRootLinkedSource<TLinkedSource, TId, TModel>(modelId, loadedReferenceContext);
            LinkReferences(rootLinkedSource, loadedReferenceContext);

            return rootLinkedSource;
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

                var rootLinkedSource = GetRootLinkedSource<TLinkedSource, TId, TModel>(modelId, loadedReferenceContext);

                var lookupIdContextLevel1 = new LookupIdContext();
                foreach (var loadLinkExpression in _loadLinkExpressions)
                {
                    loadLinkExpression.AddLookupIds(rootLinkedSource, lookupIdContextLevel1);
                }
                _referenceLoader.LoadReferences(lookupIdContextLevel1, loadedReferenceContext);
            }
            return loadedReferenceContext;
        }

        private static TLinkedSource GetRootLinkedSource<TLinkedSource, TId, TModel>(TId modelId,
            LoadedReferenceContext loadedReferenceContext) where TLinkedSource : ILinkedSource<TModel>, new()
        {
            var model = loadedReferenceContext.GetOptionalReference<TModel, TId>(modelId);

            //what if model is null

            var rootLinkedSource = new TLinkedSource();
            rootLinkedSource.Model = model;
            return rootLinkedSource;
        }

        private void LinkReferences(object linkedSource, LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkExpression in _loadLinkExpressions) {
                linkExpression.Link(linkedSource, loadedReferenceContext);
            }
        }
    }
}
