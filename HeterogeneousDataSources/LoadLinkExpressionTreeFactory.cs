using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources
{
    public class LoadLinkExpressionTreeFactory
    {
        private readonly List<ILoadLinkExpression> _loadLinkExpressions;

        public LoadLinkExpressionTreeFactory(List<ILoadLinkExpression> loadLinkExpressions){
            _loadLinkExpressions = loadLinkExpressions;
        }

        public Tree<ILoadLinkExpression> Create(ILoadLinkExpression node)
        {
            var childLoadLinkExpressions = GetChildLoadLinkExpressions(node);
            var children = childLoadLinkExpressions
                .Select(Create)
                .ToList();
            return new Tree<ILoadLinkExpression>(
                node,
                children
            );
        }

        public Type GetReferenceTypeThatCreatesACycle(ILoadLinkExpression node){
            return GetReferenceTypeThatCreatesACycle(node, new List<Type>());
        }

        private Type GetReferenceTypeThatCreatesACycle(ILoadLinkExpression node, List<Type> referenceTypesOfAncestors) {
            if(referenceTypesOfAncestors.Contains(node.ReferenceType)){ return node.ReferenceType; }

            var referenceTypesOfAncestorsForNextLevel = GetReferenceTypesOfAncestorsForNextLevel(node, referenceTypesOfAncestors);

            var childLoadLinkExpressions = GetChildLoadLinkExpressions(node);
            return childLoadLinkExpressions
                .Select(childLoadLinkExpression =>
                    GetReferenceTypeThatCreatesACycle(childLoadLinkExpression, referenceTypesOfAncestorsForNextLevel))
                .FirstOrDefault(referenceTypeThatCreateACycle=>referenceTypeThatCreateACycle!=null);
        }

        private static List<Type> GetReferenceTypesOfAncestorsForNextLevel(ILoadLinkExpression node, List<Type> referenceTypesOfAncestors)
        {
            //Make sure referenceTypesOfAncestors is not altered
            var result = referenceTypesOfAncestors.ToList();

            if (node.LoadLinkExpressionType == LoadLinkExpressionType.Root ||
                node.LoadLinkExpressionType == LoadLinkExpressionType.NestedLinkedSource)
            {
                result.Add(node.ReferenceType);
            }
            return result;
        }


        private List<ILoadLinkExpression> GetChildLoadLinkExpressions(ILoadLinkExpression node)
        {
            var nodeAsNestedLoadLinkExpression = node as INestedLoadLinkExpression;
            if (nodeAsNestedLoadLinkExpression == null) { return new List<ILoadLinkExpression>(); }

            return _loadLinkExpressions
                .Where(loadLinkExpression => 
                    loadLinkExpression.LinkedSourceType == nodeAsNestedLoadLinkExpression.ChildLinkedSourceType)
                .ToList();
        }
    }
}