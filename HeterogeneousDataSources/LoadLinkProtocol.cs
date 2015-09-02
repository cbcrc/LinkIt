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

        public TRootLinkedSource LoadLink<TRootLinkedSource>(object modelId)
        {
            if (modelId == null) { throw new ArgumentNullException("modelId");}
            EnsureRootLinedSourceTypeIsConfigured<TRootLinkedSource>();

            var loadedReferenceContext = Load<TRootLinkedSource>(modelId);
            
            LinkReferences(loadedReferenceContext);

            return (TRootLinkedSource)loadedReferenceContext.LinkedSourcesToBeBuilt
                .SingleOrDefault(linkedSource => linkedSource is TRootLinkedSource);
        }

        private void EnsureRootLinedSourceTypeIsConfigured<TRootLinkedSource>()
        {
            if (!_config.DoesLoadLinkExpressionForRootLinkedSourceTypeExists(typeof (TRootLinkedSource)))
            {
                throw new ArgumentException(
                    string.Format(
                        "{0} is not configured as a root linked source type",
                        typeof (TRootLinkedSource)
                        ),
                    "TRootLinkedSource"
                );
            }
        }

        private LoadedReferenceContext Load<TRootLinkedSource>(object modelId)
        {
            var loadedReferenceContext = new LoadedReferenceContext();
            loadedReferenceContext.AddLinkedSourcesToBeBuilt(new List<object>{modelId});
            
            using (_referenceLoader)
            {
                var numberOfLoadingLevel = _config.GetNumberOfLoadingLevel<TRootLinkedSource>();

                for (int loadingLevel = 0; loadingLevel < numberOfLoadingLevel; loadingLevel++)
                {
                    LoadNestingLevel<TRootLinkedSource>(loadingLevel, loadedReferenceContext);
                    LinkNestedLinkedSources<TRootLinkedSource>(loadingLevel, loadedReferenceContext);
                    LinkSubLinkedSources(loadedReferenceContext);
                }
            }
            return loadedReferenceContext;
        }

        private void LoadNestingLevel<TRootLinkedSource>(int loadingLevel, LoadedReferenceContext loadedReferenceContext)
        {
            var lookupIdContext = GetLookupIdContextForLoadingLevel<TRootLinkedSource>(loadingLevel, loadedReferenceContext);
            _referenceLoader.LoadReferences(lookupIdContext, loadedReferenceContext);
        }

        private LookupIdContext GetLookupIdContextForLoadingLevel<TRootLinkedSource>(int loadingLevel, LoadedReferenceContext loadedReferenceContext)
        {
            var lookupIdContext = new LookupIdContext();

            foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt){
                var loadLinkExpressions = _config.GetLoadExpressions<TRootLinkedSource>(linkedSource, loadingLevel);
                foreach (var loadLinkExpression in loadLinkExpressions){
                    loadLinkExpression.AddLookupIds(linkedSource, lookupIdContext);
                }
            }
            return lookupIdContext;
        }

        private void LinkNestedLinkedSources<TRootLinkedSource>(int loadingLevel, LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt) {
                var loadLinkExpressions = _config.GetLinkNestedLinkedSourceExpressions<TRootLinkedSource>(linkedSource, loadingLevel);
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
