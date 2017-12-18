using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Shared;

namespace LinkIt.ReferenceTrees
{
    //I wanted to reuse some open source graph library. 
    //QuickGraph is the only one I found that could do the job,
    //but it seems like I cannot release my code under MIT licence
    //if I use QuickGraph which us MS public license
    //http://www.whitesourcesoftware.com/whitesource-blog/top-10-microsoft-public-license-ms-pl-questions-answered/
    //
    //I will just code the longest path algo by myself, to avoid the dependency.
    //
    //See the link below to understand why it is important that the dependency graph is a
    //directed acyclic graphs (DAG)
    //https://en.wikipedia.org/wiki/Longest_path_problem
    public class ReferenceDependencyDagNode : IEquatable<ReferenceDependencyDagNode>
    {
        private readonly HashSet<ReferenceDependencyDagNode> _dependees = new HashSet<ReferenceDependencyDagNode>();
        private readonly HashSet<ReferenceDependencyDagNode> _dependencies = new HashSet<ReferenceDependencyDagNode>();

        public ReferenceDependencyDagNode(Type referenceType)
        {
            if (referenceType == null) throw new ArgumentNullException(nameof(referenceType));
            ReferenceType = referenceType;
        }

        public Type ReferenceType { get; }

        public int LongestDependecyChainLength
        {
            get { return DependencyPaths.Select(dependencyPath => dependencyPath.Count).Max() - 1; }
        }

        private List<List<ReferenceDependencyDagNode>> DependencyPaths
        {
            get
            {
                if (!_dependencies.Any()) return new List<List<ReferenceDependencyDagNode>> { this.Yield().ToList() };

                return _dependencies
                    .SelectMany(dependency => dependency.DependencyPaths)
                    .Select(dependencyPath => dependencyPath.Concat(this.Yield()).ToList())
                    .ToList();
            }
        }

        public bool Equals(ReferenceDependencyDagNode other)
        {
            if (other == null) return false;

            return ReferenceType == other.ReferenceType;
        }

        public void AddDependency(ReferenceDependencyDagNode dependency)
        {
            EnsureDoesNotCreateCycle(dependency);

            _dependencies.Add(dependency);
            dependency.AddDependee(this);
        }

        private void AddDependee(ReferenceDependencyDagNode dependee)
        {
            _dependees.Add(dependee);
        }

        private void EnsureDoesNotCreateCycle(ReferenceDependencyDagNode dependency)
        {
            var dependencyCycle = DetectDependencyCycle(dependency);
            if (dependencyCycle != null)
            {
                var dependencyCycleAsString = string.Join(
                    ",\n",
                    dependencyCycle.Select(item => item.ReferenceType.ToString()).ToArray()
                );

                throw new NotSupportedException(
                    $@"
Recursive load link is not supported. 
Cannot infer which reference type should be loaded first 
{ReferenceType} 
or 
{dependency.ReferenceType} 

Here is the full dependency cycle: 
{dependencyCycleAsString}
"
                );
            }
        }

        private List<ReferenceDependencyDagNode> DetectDependencyCycle(ReferenceDependencyDagNode dependency)
        {
            var dependencyPathThatCreatesCycle = dependency.DependencyPaths
                .FirstOrDefault(dependencyPath => dependencyPath.Contains(this));

            if (dependencyPathThatCreatesCycle != null)
                return dependencyPathThatCreatesCycle
                    .Concat(this.Yield())
                    .ToList();

            return null;
        }

        public override bool Equals(object obj)
        {
            var objAsContentType = obj as ReferenceDependencyDagNode;
            if (objAsContentType == null) return false;
            return Equals(objAsContentType);
        }

        public override int GetHashCode()
        {
            return ReferenceType.GetHashCode();
        }

        public override string ToString()
        {
            return ReferenceType.ToString();
        }
    }
}