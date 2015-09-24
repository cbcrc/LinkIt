using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkConfig {
        private readonly Dictionary<Type, Dictionary<int, List<Type>>> _referenceTypeByLoadingLevelByRootLinkedSourceType;
        private readonly List<ILoadLinkExpression> _allLoadLinkExpressions;

        #region Initialization
        public LoadLinkConfig(List<ILoadLinkExpression> loadLinkExpressions) {
            var linkExpressionTreeFactory = new LoadLinkExpressionTreeFactory(loadLinkExpressions);

            EnsureChildLinkedSourceTypeIsUniqueInRootLoadLinkExpressions(loadLinkExpressions);
            //EnsureNoCyclesInRootLoadLinkExpressions(loadLinkExpressions, linkExpressionTreeFactory);

            _referenceTypeByLoadingLevelByRootLinkedSourceType =
                GetReferenceTypeByLoadingLevelByRootLinkedSourceType(
                    loadLinkExpressions,
                    linkExpressionTreeFactory
                );

            _allLoadLinkExpressions = loadLinkExpressions;
        }

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

        private void EnsureChildLinkedSourceTypeIsUniqueInRootLoadLinkExpressions(List<ILoadLinkExpression> loadLinkExpressions) {
            var childLinkedSourceTypeWithDuplicates = GetRootLoadLinkExpressions(loadLinkExpressions)
                .SelectMany(rootExpression => rootExpression.ChildLinkedSourceTypes)
                .GroupBy(rootLinkedSource => rootLinkedSource)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (childLinkedSourceTypeWithDuplicates.Any()) {
                throw new ArgumentException(
                    string.Format(
                        "Can only have one root expression per child linked source, but there are many for : {0}.",
                        String.Join(",", childLinkedSourceTypeWithDuplicates)
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

        public int GetNumberOfLoadingLevel<TRootLinkedSource>()
        {
            var referenceTypeByLoadingLevel = GetReferenceTypeByLoadingLevel<TRootLinkedSource>();

            return referenceTypeByLoadingLevel.Count;
        }

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

        private Dictionary<int, List<Type>> GetReferenceTypeByLoadingLevel<TRootLinkedSource>(){
            var rootLinkedSourceType = typeof(TRootLinkedSource);
            return _referenceTypeByLoadingLevelByRootLinkedSourceType[rootLinkedSourceType];
        }
    }
}
