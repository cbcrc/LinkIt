using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkConfig {
        private readonly List<ILoadLinkExpression> _allLoadLinkExpressions;
//stle: term: rename by LoadingLevelParser
        private readonly ReferenceTypeByLoadingLevelParser _loadingLevelParser;

        #region Initialization
        public LoadLinkConfig(List<ILoadLinkExpression> loadLinkExpressions)
        {
            EnsureLoadLinkExpressionLinkTargetIdsAreUnique(loadLinkExpressions);

            var linkExpressionTreeFactory = new LoadLinkExpressionTreeFactory(loadLinkExpressions);
            _loadingLevelParser = new ReferenceTypeByLoadingLevelParser(linkExpressionTreeFactory);
            
            _allLoadLinkExpressions = loadLinkExpressions;
        }

        private void EnsureLoadLinkExpressionLinkTargetIdsAreUnique(List<ILoadLinkExpression> loadLinkExpressions) {
            var linkTargetIdsWithDuplicates = loadLinkExpressions.GetNotUniqueKey(loadLinkExpression => loadLinkExpression.LinkTargetId);

            if (linkTargetIdsWithDuplicates.Any()) {
                throw new ArgumentException(
                    string.Format(
                        "Can only have one load link expression per link target id, but there are many for : {0}.",
                        String.Join(",", linkTargetIdsWithDuplicates)
                    )
                );
            }
        }
        #endregion

        //stle: should go in protocol
        public List<ILoadLinkExpression> GetLoadExpressions(object linkedSource) {
            return GetLoadLinkExpressions(linkedSource, _allLoadLinkExpressions);
        }

        //stle: should go in protocol
        public List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, Type referenceType)
        {
            return GetLoadLinkExpressions(linkedSource, _allLoadLinkExpressions, referenceType);
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions, Type referenceType)
        {
            return GetLoadLinkExpressions(linkedSource, loadLinkExpressions)
                .Where(loadLinkExpression => loadLinkExpression.ReferenceTypes.Contains(referenceType))
                .ToList();
        }

        public List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions) {
            var linkedSourceType = linkedSource.GetType();
            return loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LinkedSourceType.Equals(linkedSourceType))
                .ToList();
        }

        public ReferenceTree CreateRootReferenceTree<TRootLinkedSource>()
        {
            var rootLinkedSourceConfig = LinkedSourceConfigs.GetConfigFor<TRootLinkedSource>();

            var rootReferenceTree = new ReferenceTree(
                rootLinkedSourceConfig.LinkedSourceModelType,
                "root",
                null
            );

            try
            {
                AddReferenceTreeForEachLinkTarget(rootLinkedSourceConfig.LinkedSourceType, rootReferenceTree);
            }
            catch (NotSupportedException ex){
                throw new NotSupportedException(
                    string.Format(
                        "Unable to create root reference tree for {0}. For more details, see inner exception.",
                        rootLinkedSourceConfig.LinkedSourceType
                    ),
                    ex
                );
            }

            return rootReferenceTree;
        }

        public void AddReferenceTreeForEachLinkTarget(Type linkedSourceType, ReferenceTree parent){
            //stle: split in two
            _allLoadLinkExpressions
                .Where(loadLinkExpression => 
                    loadLinkExpression.LinkedSourceType == linkedSourceType
                )
                .ToList()
                .ForEach(loadLinkExpression => 
                    loadLinkExpression.AddReferenceTreeForEachInclude(parent, this)
                );
        }

        public ILoadLinker<TRootLinkedSource> CreateLoadLinker<TRootLinkedSource>(IReferenceLoader referenceLoader) 
        {
            var rootLinkedSourceType = typeof(TRootLinkedSource);
            
            return LinkedSourceConfigs.GetConfigFor<TRootLinkedSource>().CreateLoadLinker(
                referenceLoader,
                GetLoadingLevelsFor<TRootLinkedSource>(),
                this
            );
        }

        private readonly Dictionary<Type, List<List<Type>>> _loadingLevelsByRootLinkedSourceType
            = new Dictionary<Type, List<List<Type>>>();

        private List<List<Type>> GetLoadingLevelsFor<TRootLinkedSource>()
        {
            var rootLinkedSourceType = typeof(TRootLinkedSource);

            //Lazy init to minimize required configuration by the client.
            //stle: dangerous for multithreading
            if (!_loadingLevelsByRootLinkedSourceType.ContainsKey(rootLinkedSourceType)){
                var loadingLevels = _loadingLevelParser.ParseLoadingLevels(rootLinkedSourceType);
                _loadingLevelsByRootLinkedSourceType.Add(rootLinkedSourceType, loadingLevels);
            }

            return _loadingLevelsByRootLinkedSourceType[rootLinkedSourceType];
        }

    }
}
