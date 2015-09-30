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

            foreach (var child in linkedExpressionTree.Children) {
                ParseTree(
                    child, 
                    GetNextLoadingLevel(node, child.Node, loadingLevel),
                    loadingLevelByReferenceType
                );
            }
        }

        private static int GetNextLoadingLevel(ILoadLinkExpression node, ILoadLinkExpression child, int loadingLevel) {
            return node.IsInDifferentLoadingLevel(child)
                ? loadingLevel + 1
                : loadingLevel;
        }

        private void ParseNode(ILoadLinkExpression node, Dictionary<Type, int> loadingLevelByReferenceType, int loadingLevel) {
            foreach (var referenceType in node.ReferenceTypes){
                var currentValue = GetLoadingLevelCurrentValue(referenceType, loadingLevelByReferenceType);
                var newValue = Math.Max(currentValue, loadingLevel);
                loadingLevelByReferenceType[referenceType] = newValue;
            }
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
