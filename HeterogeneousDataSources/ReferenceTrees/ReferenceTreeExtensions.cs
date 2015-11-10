using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.ReferenceTrees {
    public static class ReferenceTreeExtensions {
        public static List<List<Type>> ParseLoadLevels(this ReferenceTree rootReferenceTree) {
            var loadingLevelByReferenceType = new Dictionary<Type, int>();
            ParseTree(rootReferenceTree, 0, loadingLevelByReferenceType);

            return ToReferenceTypeToBeLoadedForEachLoadingLevel(loadingLevelByReferenceType);
        }

        private static void ParseTree(ReferenceTree referenceTree, int loadingLevel, Dictionary<Type, int> loadingLevelByReferenceType)
        {
            ParseNode(referenceTree.Node, loadingLevelByReferenceType, loadingLevel);

            foreach (var child in referenceTree.Children) {
                ParseTree(
                    child, 
                    loadingLevel+1,
                    loadingLevelByReferenceType
                );
            }
        }

        private static void ParseNode(ReferenceToLoad node, Dictionary<Type, int> loadingLevelByReferenceType, int loadingLevel){
            var currentValue = GetLoadingLevelCurrentValue(node.ReferenceType, loadingLevelByReferenceType);
            var newValue = Math.Max(currentValue, loadingLevel);
            loadingLevelByReferenceType[node.ReferenceType] = newValue;
        }

        private static int GetLoadingLevelCurrentValue(Type referenceType, Dictionary<Type, int> loadingLevelByReferenceType) {
            if (!loadingLevelByReferenceType.ContainsKey(referenceType)) { return -1; }
            return loadingLevelByReferenceType[referenceType];
        }

        private static List<List<Type>> ToReferenceTypeToBeLoadedForEachLoadingLevel(Dictionary<Type, int> loadingLevelByReferenceType) {
            return loadingLevelByReferenceType
                .GroupBy(
                    keySelector: p => p.Value, //loadingLevel
                    elementSelector: p => p.Key //referenceType
                )
                .OrderBy(group=>group.Key)
                .Select(group=>group.ToList())
                .ToList();
        }
    }
}
