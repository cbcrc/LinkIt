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
            _parent = parent;

            if (_parent != null){
                EnsureDoesNotCreateCycle(referenceType);
                _parent.AddChild(this);
            }
        }

        public ReferenceToLoad Node { get; private set; }
        public List<ReferenceTree> Children { get; private set; }

        private void AddChild(ReferenceTree child) {
            Children.Add(child);
        }

        private void EnsureDoesNotCreateCycle(Type referenceType)
        {
            var ancestorThatCreatesCycle = GetAncestorThatCreatesCycle(referenceType);
            //stle: better error msg
            if (ancestorThatCreatesCycle != null){
                throw new ArgumentException(
                    "cycle"
                    //string.Format(
                    //    "Some root load link expressions contain a cycle: {0}.",
                    //    String.Join(",", cycleAsString)
                    //)
                );
            }
        }

        private ReferenceTree GetAncestorThatCreatesCycle(Type referenceType)
        {
            return _parent.GetAncestorOrSelf()
                .FirstOrDefault(ancestor => ancestor.Node.ReferenceType == referenceType);
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
