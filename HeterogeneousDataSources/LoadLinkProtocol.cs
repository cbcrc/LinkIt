using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkProtocol {
        private readonly IReferenceLoader _referenceLoader;
        private readonly List<ILoadLinkExpression> _loadLinkExpressions;
        private readonly List<ILoadLinkExpression> _nestedLinkedSourceLoadLinkExpressions;
        private readonly List<ILoadLinkExpression> _referenceLoadLinkExpressions;

        public LoadLinkProtocol(IReferenceLoader referenceLoader, List<ILoadLinkExpression> loadLinkExpressions)
        {
            _referenceLoader = referenceLoader;
            _loadLinkExpressions = loadLinkExpressions;
            _nestedLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.IsNestedLinkedSourceLoadLinkExpression)
                .ToList();
            _referenceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => !loadLinkExpression.IsNestedLinkedSourceLoadLinkExpression)
                .ToList();
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

                LoadNextNestingLevel(loadedReferenceContext);
            }
            return loadedReferenceContext;
        }

        private void LoadNextNestingLevel(LoadedReferenceContext loadedReferenceContext)
        {
            var lookupIdContext = new LookupIdContext();
            foreach (var loadLinkExpression in _loadLinkExpressions)
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
            foreach (var linkExpression in _nestedLinkedSourceLoadLinkExpressions) {
                linkExpression.Link(
                    loadedReferenceContext.LinkedSourcesToBeBuilt, //stle: hum remove one arg?
                    loadedReferenceContext
                );
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
            foreach (var linkExpression in _referenceLoadLinkExpressions) {
                linkExpression.Link(
                    loadedReferenceContext.LinkedSourcesToBeBuilt,//stle: hum remove one arg?
                    loadedReferenceContext
                );
            }
        }
    }
}
