﻿using System;
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

        public Tree<ILoadLinkExpression> Create(Type rootLinkedSourceType) {
            var children = _loadLinkExpressions
                .Where(loadLinkExpression => loadLinkExpression.LinkedSourceType == rootLinkedSourceType)
                .Select(Create)
                .ToList();

            return new Tree<ILoadLinkExpression>(
                null, //no expressions at root level
                children
            );
        }

        public Type GetReferenceTypeThatCreatesACycleFromTree(ILoadLinkExpression node) {
            return GetReferenceTypeThatCreatesACycleFromTree(node, new List<Type>());
        }


        //stle: private
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

        private Type GetReferenceTypeThatCreatesACycleFromTree(ILoadLinkExpression node, List<Type> referenceTypesOfAncestors)
        {
            var referenceTypeTypeThatCreatesACycle = 
                GetReferenceTypeThatCreateACycleFromNode(node, referenceTypesOfAncestors);

            if (referenceTypeTypeThatCreatesACycle != null){ return referenceTypeTypeThatCreatesACycle; }

            return GetReferenceTypeThatCreatesACycleFromChildren(node, referenceTypesOfAncestors);
        }

        private static Type GetReferenceTypeThatCreateACycleFromNode(ILoadLinkExpression node, List<Type> referenceTypesOfAncestors) {
            return node.ReferenceTypes
                .FirstOrDefault(referenceTypesOfAncestors.Contains);
        }

        private Type GetReferenceTypeThatCreatesACycleFromChildren(ILoadLinkExpression node, List<Type> referenceTypesOfAncestors)
        {
            var referenceTypesOfAncestorsOfChildren = GetReferenceTypesOfAncestorsOfChildren(node, referenceTypesOfAncestors);

            var children = GetChildLoadLinkExpressions(node);
            return children
                .Select(child =>
                    GetReferenceTypeThatCreatesACycleFromTree(child, referenceTypesOfAncestorsOfChildren))
                .FirstOrDefault(referenceTypeThatCreateACycle => referenceTypeThatCreateACycle != null);
        }

        private static List<Type> GetReferenceTypesOfAncestorsOfChildren(ILoadLinkExpression node, List<Type> referenceTypesOfAncestors)
        {
            //Make sure referenceTypesOfAncestors is not altered
            var result = referenceTypesOfAncestors.ToList();

            result.AddRange(node.ReferenceTypes);
            return result;
        }

        private List<ILoadLinkExpression> GetChildLoadLinkExpressions(ILoadLinkExpression node)
        {
            var nodeAsNestedLoadLinkExpression = node as INestedLoadLinkExpression;
            if (nodeAsNestedLoadLinkExpression == null) { return new List<ILoadLinkExpression>(); }

            return _loadLinkExpressions
                .Where(loadLinkExpression =>
                    nodeAsNestedLoadLinkExpression.ChildLinkedSourceTypes.Contains(loadLinkExpression.LinkedSourceType))
                .ToList();
        }
    }
}