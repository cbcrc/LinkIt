using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources {
    public class ReferenceTree{
        private ReferenceTree _parent;

        //stle: review linkTargetId
        public ReferenceTree(Type referenceType, string linkTargetId, ReferenceTree parent){
            Node = new ReferenceToLoad(referenceType, linkTargetId);
            Children = new List<ReferenceTree>();
        }

        public ReferenceToLoad Node { get; private set; }
        public List<ReferenceTree> Children { get; private set; }

        public void AddChildren(List<ReferenceTree> children) {
            foreach (var child in children){
                EnsureChildDoesNotCreateCycle(child);
                child.SetParent(this);
                Children.Add(child);
            }
        }

        private void SetParent(ReferenceTree parent){
            _parent = parent;
        }

        private void EnsureChildDoesNotCreateCycle(ReferenceTree child)
        {
            var referenceThatCreatesCycle = GetReferenceThatCreatesCycle(child);
            //stle: better error msg
            if (referenceThatCreatesCycle != null){
                throw new ArgumentException(
                    "cycle"
                    //string.Format(
                    //    "Some root load link expressions contain a cycle: {0}.",
                    //    String.Join(",", cycleAsString)
                    //)
                );
            }
        }

        private ReferenceToLoad GetReferenceThatCreatesCycle(ReferenceTree child)
        {
            return GetAncestorOrSelf()
                .Select(item=>item.Node)
                .FirstOrDefault(reference => reference.ReferenceType == child.Node.ReferenceType);
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
