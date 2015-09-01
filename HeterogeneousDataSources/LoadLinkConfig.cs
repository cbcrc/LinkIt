using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class LoadLinkConfig {
        private LoadLinkExpressionTreeFactory _linkExpressionTreeFactory;

        //stle: remove fakeReferenceType
        public LoadLinkConfig(List<ILoadLinkExpression> loadLinkExpressions, List<Type>[] fakeReferenceTypeForLoadingLevel)
        {
            _linkExpressionTreeFactory = new LoadLinkExpressionTreeFactory(loadLinkExpressions);

            AllLoadLinkExpressions = loadLinkExpressions;
            ReferenceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Reference)
                .ToList();

            NestedLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => 
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.NestedLinkedSource ||
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Root
                )
                .ToList();

            SubLinkedSourceLoadLinkExpressions = loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.SubLinkedSource)
                .ToList();
        }

        //stle: should go in protocol
        public List<ILoadLinkExpression> GetLoadExpressions<TRootLinkedSource>(object linkedSource, int loadingLevel)
        {
            return GetLoadLinkExpressions<TRootLinkedSource>(linkedSource, AllLoadLinkExpressions, loadingLevel);
        }

        public List<ILoadLinkExpression> GetLinkNestedLinkedSourceExpressions<TRootLinkedSource>(object linkedSource, int loadingLevel) {
            return GetLoadLinkExpressions<TRootLinkedSource>(linkedSource, NestedLinkedSourceLoadLinkExpressions, loadingLevel);
        }

        public List<ILoadLinkExpression> GetLinkReferenceExpressions(object linkedSource) {
            return GetLoadLinkExpressions(linkedSource, ReferenceLoadLinkExpressions)
                .ToList();
        }

        public List<ILoadLinkExpression> GetSubLinkedSourceExpressions(object linkedSource) {
            return GetLoadLinkExpressions(linkedSource, SubLinkedSourceLoadLinkExpressions)
                .ToList();
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions<TRootLinkedSource>(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions, int loadingLevel) {
            var referenceTypesToBeLoaded = GetReferenceTypeForLoadingLevel<TRootLinkedSource>(loadingLevel);

            return GetLoadLinkExpressions(linkedSource, loadLinkExpressions)
                .Where(loadLinkExpression => referenceTypesToBeLoaded.Contains(loadLinkExpression.ReferenceType))
                .ToList();
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, List<ILoadLinkExpression> loadLinkExpressions) {
            var linkedSourceType = linkedSource.GetType();
            return loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LinkedSourceType.Equals(linkedSourceType))
                .ToList();
        }

        public int GetNumberOfLoadingLevel<TRootLinkedSource>()
        {
            return InferReferenceTypeByLoadingLevel<TRootLinkedSource>().Count;
        }

        public List<Type> GetReferenceTypeForLoadingLevel<TRootLinkedSource>(int loadingLevel)
        {
            return InferReferenceTypeByLoadingLevel<TRootLinkedSource>()[loadingLevel];
        }

        #region Stle: move this out
        //stle: init for performance
        public Dictionary<int, List<Type>> InferReferenceTypeByLoadingLevel<TRootLinkedSource>() {
            var rootExpression = GetRootExpression<TRootLinkedSource>();
            return InferReferenceTypeByLoadingLevel(rootExpression);
        }

        private INestedLoadLinkExpression GetRootExpression<TRootLinkedSource>() {
            var rootExpressions = AllLoadLinkExpressions
                .Where(loadLinkExpression =>
                    loadLinkExpression.LoadLinkExpressionType == LoadLinkExpressionType.Root)
                .Cast<INestedLoadLinkExpression>()
                .Where(nestedLoadLinkExpression =>
                    nestedLoadLinkExpression.ChildLinkedSourceType == typeof(TRootLinkedSource));

            //stle: assume only one RootExpression perTRootLinkedSource

            return rootExpressions.Single();
        }

        private Dictionary<int, List<Type>> InferReferenceTypeByLoadingLevel(ILoadLinkExpression rootExpression) {
            var linkedExpressionTree = _linkExpressionTreeFactory.Create(rootExpression);

            var loadingLevelByReferenceType = new Dictionary<Type, int>();
            ParseTree(linkedExpressionTree, 0, loadingLevelByReferenceType);
            return ToReferenceTypeByLoadingLevel(loadingLevelByReferenceType);
        }

        private void ParseTree(Tree<ILoadLinkExpression> linkedExpressionTree, int loadingLevel, Dictionary<Type, int> loadingLevelByReferenceType) {
            var node = linkedExpressionTree.Node;
            ParseNode(node, loadingLevelByReferenceType, loadingLevel);

            var nextLoadingLevel = GetNextLoadingLevel(node, loadingLevel);
            foreach (var child in linkedExpressionTree.Children) {
                ParseTree(child, nextLoadingLevel, loadingLevelByReferenceType);
            }
        }

        private static int GetNextLoadingLevel(ILoadLinkExpression node, int loadingLevel) {
            if (node.LoadLinkExpressionType == LoadLinkExpressionType.SubLinkedSource) { return loadingLevel; }

            return loadingLevel + 1;
        }

        private void ParseNode(ILoadLinkExpression node, Dictionary<Type, int> loadingLevelByReferenceType, int loadingLevel) {
            if (node.ReferenceType == null) { return; }

            var currentValue = GetLoadingLevelCurrentValue(node.ReferenceType, loadingLevelByReferenceType);
            var newValue = Math.Max(currentValue, loadingLevel);
            loadingLevelByReferenceType[node.ReferenceType] = newValue;
        }

        private int GetLoadingLevelCurrentValue(Type referenceType, Dictionary<Type, int> loadingLevelByReferenceType) {
            if (!loadingLevelByReferenceType.ContainsKey(referenceType)) { return -1; }
            return loadingLevelByReferenceType[referenceType];
        }

        private static Dictionary<int, List<Type>> ToReferenceTypeByLoadingLevel(Dictionary<Type, int> loadingLevelByReferenceType) {
            var referenceTypeGroupByLoadingLevel = loadingLevelByReferenceType
                .GroupBy(
                    keySelector: p => p.Value, //loadingLevel
                    elementSelector: p => p.Key, //referenceType
                    resultSelector: (key, elements) => new {
                        LoadingLevel = key,
                        ReferenceTypes = elements.ToList()
                    }
                );

            return referenceTypeGroupByLoadingLevel
                .ToDictionary(
                    p => p.LoadingLevel,
                    p => p.ReferenceTypes
                );
        } 
        #endregion

        private List<ILoadLinkExpression> AllLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> ReferenceLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> NestedLinkedSourceLoadLinkExpressions { get; set; }
        private List<ILoadLinkExpression> SubLinkedSourceLoadLinkExpressions { get; set; }
    }
}
