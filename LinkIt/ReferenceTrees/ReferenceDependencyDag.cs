using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.ReferenceTrees
{
    public class ReferenceDependencyDag
    {
        private readonly Dictionary<Type, ReferenceDependencyDagNode> _nodeByReferenceType = new Dictionary<Type, ReferenceDependencyDagNode>();

        public List<List<Type>> ReferenceTypeToBeLoadedForEachLoadingLevel()
        {
            return _nodeByReferenceType.Values
                .GroupBy(
                    p => p.LongestDependecyChainLength, //loadingLevel
                    p => p.ReferenceType
                )
                .OrderBy(group => group.Key)
                .Select(group => group.ToList())
                .ToList();
        }

        #region Construction

        public ReferenceDependencyDag(Type root)
        {
            AddNodeIfDoesNotExist(root);
        }

        public void AddDependency(Type from, Type to)
        {
            if (GetNode(to) == null) throw new ArgumentException("Cannot add dependency to reference type not yet in the graph", nameof(to));

            AddNodeIfDoesNotExist(from);

            var fromNode = GetNode(from);
            var toNode = GetNode(to);
            fromNode.AddDependency(toNode);
        }

        private ReferenceDependencyDagNode GetNode(Type referenceType)
        {
            return _nodeByReferenceType[referenceType];
        }

        private void AddNodeIfDoesNotExist(Type referenceType)
        {
            if (!_nodeByReferenceType.ContainsKey(referenceType))
                _nodeByReferenceType.Add(
                    referenceType,
                    new ReferenceDependencyDagNode(referenceType)
                );
        }

        #endregion
    }
}