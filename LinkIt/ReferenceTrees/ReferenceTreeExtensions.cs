using System;
using System.Collections.Generic;

namespace LinkIt.ReferenceTrees {
    public static class ReferenceTreeExtensions {
        public static List<List<Type>> ParseLoadingLevels(this ReferenceTree rootReferenceTree)
        {
            var referenceDependencyDag = CreateReferenceDependencyDag(rootReferenceTree);
            return referenceDependencyDag.ReferenceTypeToBeLoadedForEachLoadingLevel();
        }

        private static ReferenceDependencyDag CreateReferenceDependencyDag(ReferenceTree rootReferenceTree) {
            var referenceDependencyDag = new ReferenceDependencyDag(rootReferenceTree.Node.ReferenceType);
            AddDependencyFromChildrenToParent(referenceDependencyDag, rootReferenceTree);
            return referenceDependencyDag;
        }

        private static void AddDependencyFromChildrenToParent(ReferenceDependencyDag referenceDependencyDag, ReferenceTree parent) {
            foreach (var child in parent.Children) {
                referenceDependencyDag.AddDependency(
                    from: child.Node.ReferenceType,
                    to: parent.Node.ReferenceType
                );
                AddDependencyFromChildrenToParent(referenceDependencyDag, child);
            }
        }
    }
}
