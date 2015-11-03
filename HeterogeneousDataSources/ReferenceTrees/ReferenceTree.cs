using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources {
    public class ReferenceTree{
        private readonly ReferenceTree _parent;

        //stle: review linkTargetId
        public ReferenceTree(Type referenceType, string linkTargetId, ReferenceTree parent){
            Node = new ReferenceToLoad(referenceType, linkTargetId);
            Children = new List<ReferenceTree>();

            if (parent != null){
                EnsureDoesNotCreateCycle(parent);

                _parent = parent;
                _parent.AddChild(this);
            }
        }

        public ReferenceToLoad Node { get; private set; }
        public List<ReferenceTree> Children { get; private set; }

        private void AddChild(ReferenceTree child){
            Children.Add(child);
        }

        private void EnsureDoesNotCreateCycle(ReferenceTree parent)
        {
            var nodeThatCreatesCycle = GetNodeThatCreatesCycle(parent);
            if (nodeThatCreatesCycle != null){
                throw new NotSupportedException(
                    string.Format(
                        "Recursive load link is not supported. The cycle occurs between {0} and {1} for the reference of type {2}.",
                        nodeThatCreatesCycle.LinkTargetId,
                        Node.LinkTargetId,
                        Node.ReferenceType
                    )
                );
            }
        }

        private ReferenceToLoad GetNodeThatCreatesCycle(ReferenceTree parent)
        {
            if (parent == null) { return null; }

            return parent.GetAncestorOrSelf()
                .Select(item=>item.Node)
                .FirstOrDefault(reference => reference.ReferenceType == Node.ReferenceType);
        }

        private List<ReferenceTree> GetAncestorOrSelf()
        {
            var ancestors = _parent != null
                ? _parent.GetAncestorOrSelf()
                : new List<ReferenceTree>();

            var self = new List<ReferenceTree> { this };

            return ancestors.Concat(self).ToList();
        }
    }
}
