using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkConfig {
        private readonly Dictionary<Type, Dictionary<int, List<Type>>> _referenceTypeByLoadingLevelByRootLinkedSourceType;
        private readonly List<ILoadLinkExpression> _allLoadLinkExpressions;
        
        //stle: delete this
        private LoadLinkExpressionTreeFactory _tempLinkExpressionTreeFactory;

        #region Initialization
        public LoadLinkConfig(List<ILoadLinkExpression> loadLinkExpressions)
        {
            EnsureLoadLinkExpressionLinkTargetIdsAreUnique(loadLinkExpressions);

            var linkExpressionTreeFactory = new LoadLinkExpressionTreeFactory(loadLinkExpressions);
            _tempLinkExpressionTreeFactory = linkExpressionTreeFactory;
            
            //EnsureNoCyclesInRootLoadLinkExpressions(loadLinkExpressions, linkExpressionTreeFactory);

            _referenceTypeByLoadingLevelByRootLinkedSourceType =
                GetReferenceTypeByLoadingLevelByRootLinkedSourceType(
                    loadLinkExpressions,
                    linkExpressionTreeFactory
                );

            _allLoadLinkExpressions = loadLinkExpressions;
        }

        //stle: adapt this
        private Dictionary<Type, Dictionary<int, List<Type>>> GetReferenceTypeByLoadingLevelByRootLinkedSourceType(List<ILoadLinkExpression> loadLinkExpressions, LoadLinkExpressionTreeFactory linkExpressionTreeFactory) {
            var parser = new ReferenceTypeByLoadingLevelParser(linkExpressionTreeFactory);

            return GetRootLoadLinkExpressions(loadLinkExpressions)
                .Select(rootExpression =>
                    new {
                        RootLinkedSourceType = rootExpression.ChildLinkedSourceTypes.Single(),
                        ReferenceTypeByLoadingLevel = parser.ParseReferenceTypeByLoadingLevel(rootExpression)
                    }
                )
                .ToDictionary(
                    p => p.RootLinkedSourceType,
                    p => p.ReferenceTypeByLoadingLevel
                );
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
            var cycles = GetRootLoadLinkExpressions(loadLinkExpressions)
                .Select(
                    rootLoadLinkExpression =>
                        new {
                            RootLoadLinkExpression = rootLoadLinkExpression,
                            ReferenceTypeThatCreatesACycle = loadLinkExpressionTreeFactory.GetReferenceTypeThatCreatesACycleFromTree(rootLoadLinkExpression)
                        })
                .Where(potentialCycle => potentialCycle.ReferenceTypeThatCreatesACycle != null)
                .ToList();

            if (cycles.Any()) {
                var cycleAsString = cycles
                    .Select(cycle => string.Format("{0} creates a cycle in {1}", cycle.ReferenceTypeThatCreatesACycle, cycle.RootLoadLinkExpression.ChildLinkedSourceTypes))
                    .ToList();

                throw new ArgumentException(
                    string.Format(
                        "Some root load link expressions contain a cycle: {0}.",
                        String.Join(",", cycleAsString)
                    ),
                    "loadLinkExpressions"
                );
            }
        }

        private List<INestedLoadLinkExpression> GetRootLoadLinkExpressions(List<ILoadLinkExpression> loadLinkExpressions) {
            return loadLinkExpressions
                .Where(loadLinkExpression =>
                    loadLinkExpression.TempIsRoot)
                .Cast<INestedLoadLinkExpression>()
                .ToList();
        } 
        #endregion

        public int GetNumberOfLoadingLevel2<TRootLinkedSource>() {
            var referenceTypeByLoadingLevel = GetReferenceTypeByLoadingLevel2<TRootLinkedSource>();

            return referenceTypeByLoadingLevel.Count;
        }

        public List<Type> GetReferenceTypeToBeLoaded2<TRootLinkedSource>(int loadingLevel) {
            var referenceTypeByLoadingLevel = GetReferenceTypeByLoadingLevel2<TRootLinkedSource>();

            return referenceTypeByLoadingLevel[loadingLevel];
        }
        
        //stle: still required?
        public int GetNumberOfLoadingLevel<TRootLinkedSource>()
        {
            var referenceTypeByLoadingLevel = GetReferenceTypeByLoadingLevel<TRootLinkedSource>();

            return referenceTypeByLoadingLevel.Count;
        }

        //stle: still required?
        public List<Type> GetReferenceTypeToBeLoaded<TRootLinkedSource>(int loadingLevel) {
            var referenceTypeByLoadingLevel = GetReferenceTypeByLoadingLevel<TRootLinkedSource>();

            return referenceTypeByLoadingLevel[loadingLevel];
        }

        //stle: should go in protocol
        public List<ILoadLinkExpression> GetLoadExpressions(object linkedSource) {
            return GetLoadLinkExpressions(linkedSource, _allLoadLinkExpressions);
        }

        //stle: should go in protocol
        public List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, Type referenceType)
        {
            return GetLoadLinkExpressions(linkedSource, _allLoadLinkExpressions, referenceType);
        }

        public bool DoesLoadLinkExpressionForRootLinkedSourceTypeExists(Type rootLinkedSourceType)
        {
            return _referenceTypeByLoadingLevelByRootLinkedSourceType.ContainsKey(rootLinkedSourceType);
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

        //stle: still required?
        private Dictionary<int, List<Type>> GetReferenceTypeByLoadingLevel<TRootLinkedSource>(){
            var rootLinkedSourceType = typeof(TRootLinkedSource);
            return _referenceTypeByLoadingLevelByRootLinkedSourceType[rootLinkedSourceType];
        }

        private Dictionary<int, List<Type>> GetReferenceTypeByLoadingLevel2<TRootLinkedSource>() {
            var parser = new ReferenceTypeByLoadingLevelParser(_tempLinkExpressionTreeFactory);
            return parser.ParseReferenceTypeByLoadingLevel<TRootLinkedSource>();
        }

    }
}
