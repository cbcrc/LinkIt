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

        public TRootLinkedSource LoadLinkModel<TRootLinkedSource, TRootLinkedSourceModel>(TRootLinkedSourceModel model)
            where TRootLinkedSource : class, ILinkedSource<TRootLinkedSourceModel>, new() 
        {
            //stle: make LinkedSource a base class???

            //stle: sub linked source

            //stle: dry

            var loadedReferenceContext = new LoadedReferenceContext();
            var rootLinkedSource = LoadLinkExpressionUtil.CreateLinkedSource<TRootLinkedSource, TRootLinkedSourceModel>(
                model,
                loadedReferenceContext
            );
            LinkSubLinkedSources(loadedReferenceContext);

            Load2<TRootLinkedSource>(loadedReferenceContext);

            LinkReferences(loadedReferenceContext);

            return rootLinkedSource;
        }

        public TRootLinkedSource LoadLinkWithoutRoot<TRootLinkedSource, TRootLinkedSourceModel, TId>(TId modelId)
            where TRootLinkedSource : class, ILinkedSource<TRootLinkedSourceModel>, new() 
        {
            var lookupIdContext = new LookupIdContext();
            lookupIdContext.AddSingle<TRootLinkedSourceModel>(modelId);
            var loadedReferenceContext = new LoadedReferenceContext();

            _referenceLoader.LoadReferences(lookupIdContext, loadedReferenceContext);

            var model = loadedReferenceContext.GetOptionalReference<TRootLinkedSourceModel, TId>(modelId);
            return LoadLinkModel<TRootLinkedSource, TRootLinkedSourceModel>(model);
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

        //stle dry
        private void Load2<TRootLinkedSource>(LoadedReferenceContext loadedReferenceContext)
        {
            using (_referenceLoader) {
                var numberOfLoadingLevel = _config.GetNumberOfLoadingLevel2<TRootLinkedSource>();

                for (int loadingLevel = 0; loadingLevel < numberOfLoadingLevel; loadingLevel++) {
                    var referenceTypeToBeLoaded = _config.GetReferenceTypeToBeLoaded2<TRootLinkedSource>(loadingLevel);

                    LoadNestingLevel(loadedReferenceContext, referenceTypeToBeLoaded);
                    LinkNestedLinkedSources(loadedReferenceContext, referenceTypeToBeLoaded);
                    LinkSubLinkedSources(loadedReferenceContext);
                }
            }
        }


        private LoadedReferenceContext Load<TRootLinkedSource>(object modelId)
        {
            var loadedReferenceContext = new LoadedReferenceContext();
            loadedReferenceContext.AddLinkedSourceToBeBuilt(modelId);
            
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
