﻿using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources {
    public class ReferenceTypeByLoadingLevelParser {
        private readonly LoadLinkExpressionTreeFactory _linkExpressionTreeFactory;

        public ReferenceTypeByLoadingLevelParser(LoadLinkExpressionTreeFactory linkExpressionTreeFactory)
        {
            _linkExpressionTreeFactory = linkExpressionTreeFactory;
        }

        public List<List<Type>> ParseLoadingLevels(Type rootLinkedSourceType) {
            var linkedExpressionTree = _linkExpressionTreeFactory.Create(rootLinkedSourceType);

            var loadingLevelByReferenceType = new Dictionary<Type, int>();
            ParseTree(linkedExpressionTree, 0, loadingLevelByReferenceType);

            return ToReferenceTypeToBeLoadedForEachLoadingLevel(loadingLevelByReferenceType);
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

        private static int GetNextLoadingLevel(ILoadLinkExpression node, ILoadLinkExpression child, int loadingLevel){
            if (node == null){ return loadingLevel; }

            return node.IsInDifferentLoadingLevel(child)
                ? loadingLevel + 1
                : loadingLevel;
        }

        private void ParseNode(ILoadLinkExpression node, Dictionary<Type, int> loadingLevelByReferenceType, int loadingLevel)
        {
            if (node == null) { return; }

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

        private List<List<Type>> ToReferenceTypeToBeLoadedForEachLoadingLevel(Dictionary<Type, int> loadingLevelByReferenceType) {
            return loadingLevelByReferenceType
                .GroupBy(
                    keySelector: p => p.Value, //loadingLevel
                    elementSelector: p => p.Key, //referenceType
                    resultSelector: (key, elements) => elements.ToList()
                )
                .ToList();
        }
    }
}