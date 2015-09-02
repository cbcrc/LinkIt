using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class ReferenceTypeByLoadingLevelParser {
        private LoadLinkExpressionTreeFactory _linkExpressionTreeFactory;

        public ReferenceTypeByLoadingLevelParser(LoadLinkExpressionTreeFactory linkExpressionTreeFactory)
        {
            _linkExpressionTreeFactory = linkExpressionTreeFactory;
        }

        public Dictionary<int, List<Type>> ParseReferenceTypeByLoadingLevel(ILoadLinkExpression rootExpression) {
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
    }
}
