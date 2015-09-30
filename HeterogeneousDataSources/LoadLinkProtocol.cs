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

        //stle: dry
        public TRootLinkedSource LoadLinkModel<TRootLinkedSource>(TRootLinkedSource linkedSource)
        {
            //stle: beaviour on model null? and id null

            EnsureRootLinedSourceTypeIsConfigured<TRootLinkedSource>();

            var loadedReferenceContext = new LoadedReferenceContext();
            loadedReferenceContext.AddLinkedSourceToBeBuilt(linkedSource);
            LinkSubLinkedSources(loadedReferenceContext);

            Load<TRootLinkedSource>(loadedReferenceContext, 1);

            LinkReferences(loadedReferenceContext);
            
            return linkedSource;
        }


        public TRootLinkedSource LoadLink<TRootLinkedSource>(object modelId)
        {
            if (modelId == null) { throw new ArgumentNullException("modelId");}
            EnsureRootLinedSourceTypeIsConfigured<TRootLinkedSource>();

            var loadedReferenceContext = new LoadedReferenceContext();
            loadedReferenceContext.AddLinkedSourceToBeBuilt(modelId);

            Load<TRootLinkedSource>(loadedReferenceContext, 0);
            
            //stle: dry
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

        private void Load<TRootLinkedSource>(LoadedReferenceContext loadedReferenceContext, int startAtLoadingLevel)
        {
            using (_referenceLoader)
            {
                var numberOfLoadingLevel = _config.GetNumberOfLoadingLevel<TRootLinkedSource>();

                for (int loadingLevel = startAtLoadingLevel; loadingLevel < numberOfLoadingLevel; loadingLevel++) {
                    var referenceTypeToBeLoaded = _config.GetReferenceTypeToBeLoaded<TRootLinkedSource>(loadingLevel);

                    LoadNestingLevel(loadedReferenceContext, referenceTypeToBeLoaded);
                    LinkNestedLinkedSources(loadedReferenceContext, referenceTypeToBeLoaded);
                    LinkSubLinkedSources(loadedReferenceContext);
                }
            }
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
                    var loadLinkExpressions = _config.GetLoadLinkExpressions(linkedSource, referenceTypeToBeLoaded);
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
                    var loadLinkExpressions = _config.GetLoadLinkExpressions(linkedSource, referenceTypeToBeLoaded);
                    foreach (var loadLinkExpression in loadLinkExpressions)
                    {
                        loadLinkExpression.LinkNestedLinkedSource(linkedSource, loadedReferenceContext, referenceTypeToBeLoaded);
                    }
                }
            }
        }

        private void LinkSubLinkedSources(LoadedReferenceContext loadedReferenceContext) {
            while (loadedReferenceContext.GetLinkedSourceWithSubLinkedSourceToLink().Any()){
                foreach (var linkedSource in loadedReferenceContext.GetLinkedSourceWithSubLinkedSourceToLink()){
                    var loadLinkExpressions = _config.GetLoadExpressions(linkedSource);
                    foreach (var loadLinkExpression in loadLinkExpressions)
                    {
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
    }
}
