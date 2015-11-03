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
            //stle: better error msg
            if (nodeThatCreatesCycle != null){
                throw new ArgumentException(
                    "cycle"
                    //string.Format(
                    //    "Some root load link expressions contain a cycle: {0}.",
                    //    String.Join(",", cycleAsString)
                    //)
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
