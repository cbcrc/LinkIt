using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkConfig {
        private readonly Dictionary<Type, Dictionary<int, List<Type>>> _referenceTypeByLoadingLevelByRootLinkedSourceType;
        private readonly List<ILoadLinkExpression> _allLoadLinkExpressions;
        private readonly List<List<Type>> _fake;
        private readonly List<ILoadLinkExpression> _referenceLoadLinkExpressions;
        private readonly List<ILoadLinkExpression> _nestedLinkedSourceLoadLinkExpressions;
        private readonly List<ILoadLinkExpression> _subLinkedSourceLoadLinkExpressions;

        #region Initialization
        public LoadLinkConfig(List<ILoadLinkExpression> loadLinkExpressions, List<List<Type>> fake=null) {
//            var linkExpressionTreeFactory = new LoadLinkExpressionTreeFactory(loadLinkExpressions);

            EnsureChildLinkedSourceTypeIsUniqueInRootLoadLinkExpressions(loadLinkExpressions);

            //stle: temp
            _fake = fake;
            //EnsureNoCyclesInRootLoadLinkExpressions(loadLinkExpressions, linkExpressionTreeFactory);

            //_referenceTypeByLoadingLevelByRootLinkedSourceType =
            //    GetReferenceTypeByLoadingLevelByRootLinkedSourceType(
            //        loadLinkExpressions,
            //        linkExpressionTreeFactory
            //    );

            _allLoadLinkExpressions = loadLinkExpressions;

            _referenceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Reference)
                .ToList();

            _nestedLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression =>
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.NestedLinkedSource ||
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Root
                )
                .ToList();

            _subLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.SubLinkedSource)
                .ToList();
        }

/*        private Dictionary<Type, Dictionary<int, List<Type>>> GetReferenceTypeByLoadingLevelByRootLinkedSourceType(List<ILoadLinkExpression> loadLinkExpressions, LoadLinkExpressionTreeFactory linkExpressionTreeFactory) {
            var parser = new ReferenceTypeByLoadingLevelParser(linkExpressionTreeFactory);

            return GetRootLoadLinkExpressions(loadLinkExpressions)
                .Select(rootExpression =>
                    new {
                        RootLinkedSourceType = rootExpression.ChildLinkedSourceType,
                        ReferenceTypeByLoadingLevel = parser.ParseReferenceTypeByLoadingLevel(rootExpression)
                    }
                )
                .ToDictionary(
                    p => p.RootLinkedSourceType,
                    p => p.ReferenceTypeByLoadingLevel
                );
        }
 */ 

        private void EnsureChildLinkedSourceTypeIsUniqueInRootLoadLinkExpressions(List<ILoadLinkExpression> loadLinkExpressions) {
            var childLinkedSourceTypeWithDuplicates = GetRootLoadLinkExpressions(loadLinkExpressions)
                .GroupBy(rootExpression => rootExpression.ChildLinkedSourceType)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (childLinkedSourceTypeWithDuplicates.Any()) {
                throw new ArgumentException(
                    string.Format(
                        "Can only have one root expression per child linked source, but there are many for : {0}.",
                        String.Join(",", childLinkedSourceTypeWithDuplicates),
                        "loadLinkExpressions"
                    )
                );
            }
        }
/*
        private void EnsureNoCyclesInRootLoadLinkExpressions(List<ILoadLinkExpression> loadLinkExpressions, LoadLinkExpressionTreeFactory loadLinkExpressionTreeFactory) {
            var cycles = GetRootLoadLinkExpressions(loadLinkExpressions)
                .Select(
                    rootLoadLinkExpression =>
                        new {
                            RootLoadLinkExpression = rootLoadLinkExpression,
                            ReferenceTypeThatCreatesACycle = loadLinkExpressionTreeFactory.GetReferenceTypeThatCreatesACycle(rootLoadLinkExpression)
                        })
                .Where(potentialCycle => potentialCycle.ReferenceTypeThatCreatesACycle != null)
                .ToList();

            if (cycles.Any()) {
                var cycleAsString = cycles
                    .Select(cycle => string.Format("{0} creates a cycle in {1}", cycle.ReferenceTypeThatCreatesACycle, cycle.RootLoadLinkExpression.ChildLinkedSourceType))
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
*/
        private List<INestedLoadLinkExpression> GetRootLoadLinkExpressions(List<ILoadLinkExpression> loadLinkExpressions) {
            return loadLinkExpressions
                .Where(loadLinkExpression =>
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Root)
                .Cast<INestedLoadLinkExpression>()
                .ToList();
        } 
        #endregion

        public int GetNumberOfLoadingLevel<TRootLinkedSource>()
        {
            return _fake.Count;
            //var referenceTypeByLoadingLevel = GetReferenceTypeByLoadingLevel<TRootLinkedSource>();

            //return referenceTypeByLoadingLevel.Count;
        }

        public List<Type> GetReferenceTypeToBeLoaded<TRootLinkedSource>(int loadingLevel) {
            return _fake[loadingLevel];

            //var referenceTypeByLoadingLevel = GetReferenceTypeByLoadingLevel<TRootLinkedSource>();

            //return referenceTypeByLoadingLevel[loadingLevel];
        }


        //stle: should go in protocol
        public List<ILoadLinkExpression> GetLoadExpressions(object linkedSource, Type referenceType)
        {
            return GetLoadLinkExpressions(linkedSource, _allLoadLinkExpressions, referenceType);
        }

        public List<ILoadLinkExpression> GetLinkNestedLinkedSourceExpressions(object linkedSource, Type referenceType) {
            return GetLoadLinkExpressions(linkedSource, _nestedLinkedSourceLoadLinkExpressions, referenceType);
        }

        public List<ILoadLinkExpression> GetLinkReferenceExpressions(object linkedSource) {
            return GetLoadLinkExpressions(linkedSource, _referenceLoadLinkExpressions)
                .ToList();
        }

        public List<ILoadLinkExpression> GetSubLinkedSourceExpressions(object linkedSource) {
            return GetLoadLinkExpressions(linkedSource, _subLinkedSourceLoadLinkExpressions)
                .ToList();
        }

        public bool DoesLoadLinkExpressionForRootLinkedSourceTypeExists(Type rootLinkedSourceType)
        {
            return true;
//            return _referenceTypeByLoadingLevelByRootLinkedSourceType.ContainsKey(rootLinkedSourceType);
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions, Type referenceType)
        {
            return GetLoadLinkExpressions(linkedSource, loadLinkExpressions)
                .Where(loadLinkExpression => loadLinkExpression.ReferenceTypes.Contains(referenceType))
                .ToList();
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions) {
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
