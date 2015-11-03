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
            
            //EnsureNoCyclesInRootLoadLinkExpressions(loadLinkExpressions, linkExpressionTreeFactory);

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

        private void EnsureNoCyclesInRootLoadLinkExpressions(List<ILoadLinkExpression> loadLinkExpressions, LoadLinkExpressionTreeFactory loadLinkExpressionTreeFactory) {
            //var cycles = GetRootLoadLinkExpressions(loadLinkExpressions)
            //    .Select(
            //        rootLoadLinkExpression =>
            //            new {
            //                RootLoadLinkExpression = rootLoadLinkExpression,
            //                ReferenceTypeThatCreatesACycle = loadLinkExpressionTreeFactory.GetReferenceTypeThatCreatesACycleFromTree(rootLoadLinkExpression)
            //            })
            //    .Where(potentialCycle => potentialCycle.ReferenceTypeThatCreatesACycle != null)
            //    .ToList();

            //if (cycles.Any()) {
            //    var cycleAsString = cycles
            //        .Select(cycle => string.Format("{0} creates a cycle in {1}", cycle.ReferenceTypeThatCreatesACycle, cycle.RootLoadLinkExpression.ChildLinkedSourceTypes))
            //        .ToList();

            //    throw new ArgumentException(
            //        string.Format(
            //            "Some root load link expressions contain a cycle: {0}.",
            //            String.Join(",", cycleAsString)
            //        ),
            //        "loadLinkExpressions"
            //    );
            //}
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

            var referenceTree = new ReferenceTree(
                rootLinkedSourceConfig.LinkedSourceModelType,
                "root",
                null
            );

            referenceTree.AddChildren(
                CreateReferenceTreeForEachLinkTarget(rootLinkedSourceConfig.LinkedSourceType, referenceTree)
            );

            return referenceTree;
        }

        public List<ReferenceTree> CreateReferenceTreeForEachLinkTarget(Type linkedSourceType, ReferenceTree parent) {
            return _allLoadLinkExpressions
                    .Where(loadLinkExpression => loadLinkExpression.LinkedSourceType == linkedSourceType)
                    .SelectMany(loadLinkExpression => loadLinkExpression.CreateReferenceTreeForEachInclude(parent, this))
                    .ToList();
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
