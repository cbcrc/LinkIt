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

        public TLinkedSource LoadLink<TLinkedSource>(object modelId)
        {
            if (modelId == null) { throw new ArgumentNullException("modelId");}

            var loadedReferenceContext = Load<TLinkedSource>(modelId);
            
            LinkReferences(loadedReferenceContext);

            return (TLinkedSource)loadedReferenceContext.LinkedSourcesToBeBuilt
                .SingleOrDefault(linkedSource => linkedSource is TLinkedSource);
        }

        private LoadedReferenceContext Load<TLinkedSource>(object modelId)
        {
            var loadedReferenceContext = new LoadedReferenceContext();
            loadedReferenceContext.AddLinkedSourcesToBeBuilt(new List<object>{modelId});
            
            using (_referenceLoader)
            {
                var numberOfLoadingLevel = _config.GetNumberOfLoadingLevel<TLinkedSource>();

                for (int loadingLevel = 0; loadingLevel < numberOfLoadingLevel; loadingLevel++)
                {
                    LoadNestingLevel<TLinkedSource>(loadingLevel, loadedReferenceContext);
                    LinkNestedLinkedSources<TLinkedSource>(loadingLevel, loadedReferenceContext);
                    LinkSubLinkedSources(loadedReferenceContext);
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

        private void LinkNestedLinkedSources<TLinkedSource>(int loadingLevel, LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt) {
                var loadLinkExpressions = _config.GetLinkNestedLinkedSourceExpressions<TLinkedSource>(linkedSource, loadingLevel);
                foreach (var loadLinkExpression in loadLinkExpressions) {
                    loadLinkExpression.Link(linkedSource, loadedReferenceContext);
                }
            }
        }

        private void LinkSubLinkedSources(LoadedReferenceContext loadedReferenceContext) {
            while (loadedReferenceContext.GetLinkedSourceWithSubLinkedSourceToLink().Any()){
                foreach (var linkedSource in loadedReferenceContext.GetLinkedSourceWithSubLinkedSourceToLink()){
                    var loadLinkExpressions = _config.GetSubLinkedSourceExpressions(linkedSource);
                    foreach (var loadLinkExpression in loadLinkExpressions)
                    {
                        loadLinkExpression.Link(linkedSource, loadedReferenceContext);
                    }
                    loadedReferenceContext.AddLinkedSourceWhereSubLinkedSourceAreLinked(linkedSource);
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
    }
}
