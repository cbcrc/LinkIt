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

                for (int loadingLevel = 0; loadingLevel < numberOfLoadingLevel; loadingLevel++){
                    var referenceTypeToBeLoaded = _config.GetReferenceTypeToBeLoaded<TRootLinkedSource>(loadingLevel);

                    LoadNestingLevel(loadedReferenceContext, referenceTypeToBeLoaded);
                    LinkNestedLinkedSources(loadedReferenceContext, referenceTypeToBeLoaded);
                    LinkSubLinkedSources(loadedReferenceContext);
                }
            }
            return loadedReferenceContext;
        }

        private void LoadNestingLevel(LoadedReferenceContext loadedReferenceContext, List<Type> referenceTypeToBeLoaded)
        {
            var lookupIdContext = GetLookupIdContextForLoadingLevel(loadedReferenceContext, referenceTypeToBeLoaded);
            _referenceLoader.LoadReferences(lookupIdContext, loadedReferenceContext);
        }

        private LookupIdContext GetLookupIdContextForLoadingLevel(LoadedReferenceContext loadedReferenceContext, List<Type> referenceTypesToBeLoaded)
        {
            var lookupIdContext = new LookupIdContext();

            foreach (var referenceTypeToBeLoaded in referenceTypesToBeLoaded){
                foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt) {
                    var loadLinkExpressions = _config.GetLoadExpressions(linkedSource, referenceTypeToBeLoaded);
                    foreach (var loadLinkExpression in loadLinkExpressions) {
                        loadLinkExpression.AddLookupIds(linkedSource, lookupIdContext, referenceTypeToBeLoaded);
                    }
                }
            }
            return lookupIdContext;
        }

        private void LinkNestedLinkedSources(LoadedReferenceContext loadedReferenceContext, List<Type> referenceTypesToBeLoaded) {
            foreach (var referenceTypeToBeLoaded in referenceTypesToBeLoaded){
                foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt)
                {
                    var loadLinkExpressions = _config.GetLinkNestedLinkedSourceExpressions(linkedSource, referenceTypeToBeLoaded);
                    foreach (var loadLinkExpression in loadLinkExpressions)
                    {
                        loadLinkExpression.Link(linkedSource, loadedReferenceContext, referenceTypeToBeLoaded);
                    }
                }
            }
        }

        private void LinkSubLinkedSources(LoadedReferenceContext loadedReferenceContext) {
            while (loadedReferenceContext.GetLinkedSourceWithSubLinkedSourceToLink().Any()){
                foreach (var linkedSource in loadedReferenceContext.GetLinkedSourceWithSubLinkedSourceToLink()){
                    var loadLinkExpressions = _config.GetSubLinkedSourceExpressions(linkedSource);
                    foreach (var loadLinkExpression in loadLinkExpressions)
                    {
                        loadLinkExpression.Link(
                            linkedSource, 
                            loadedReferenceContext, 
                            null //stle: do not care it that context: the interface sucks!
                        );
                    }
                    loadedReferenceContext.AddLinkedSourceWhereSubLinkedSourceAreLinked(linkedSource);
                }
            }
        }

        private void LinkReferences(LoadedReferenceContext loadedReferenceContext) {
            foreach (var linkedSource in loadedReferenceContext.LinkedSourcesToBeBuilt) {
                var loadLinkExpressions = _config.GetLinkReferenceExpressions(linkedSource);
                foreach (var loadLinkExpression in loadLinkExpressions) {
                    loadLinkExpression.Link(
                        linkedSource, 
                        loadedReferenceContext, 
                        null //stle:not required in that context, the interface sucks!
                    );
                }
            }
        }
    }
}
