// Copyright (c) 2016 Martin Evans
// Source: https://github.com/martindevans/TopologicalSorting
// Modified by Radio-Canada for the purposes of this library.

using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.TopologicalSorting
{
    /// <summary>
    /// A graph of object dependencies from which a topological sort can be extracted
    /// </summary>
    internal class DependencyGraph
    {
        private readonly Dictionary<DependencyType, Dependency> _dependencies = new Dictionary<DependencyType, Dependency>();

        /// <summary>
        /// Gets the dependencies which are part of this dependency graph
        /// </summary>
        /// <value>The dependencies.</value>
        public IEnumerable<Dependency> Dependencies => _dependencies.Values;

        /// <summary>
        /// Gets the dependency count.
        /// </summary>
        /// <value>The dependency count.</value>
        public int DependencyCount => _dependencies.Count;

        public Dependency GetOrAdd(Type modelType, Type linkedSourceType = null)
        {
            var type = new DependencyType(modelType, linkedSourceType);
            return GetOrAdd(type);
        }

        public Dependency GetOrAdd(DependencyType type)
        {
            if (_dependencies.TryGetValue(type, out var dependency))
            {
                return dependency;
            }

            dependency = new Dependency(this, type);
            _dependencies.Add(type, dependency);

            return dependency;
        }

        /// <summary>
        /// Reduce the graph, merging any and all dependecies of a model type, as long as no cycle is created.
        /// </summary>
        public void Reduce()
        {
            // Group dependencies of the same model type
            var topologicalSorts = Dependencies.GroupBy(d => d.Type.ModelType)
                .Select(g => g.ToList())
                .Where(d => d.Count > 1)
                // Process those with the most dependencies first
                .OrderByDescending(d => d.Count)
                // Create a graph with only dependencies with the same model type
                .Select(CreateSimplifiedGraph)
                // Sort them topologically
                .Select(TopologicalSort.For);

            foreach (var topologicalSort in topologicalSorts)
            {
                // Merge each level of the topoligical sort into one dependency
                foreach (var set in topologicalSort.DependencySets)
                {
                    var originalDependencies = set.Select(d => _dependencies[d.Type]);
                    MergeDependencies(originalDependencies);
                }
            }
        }

        public TopologicalSort Sort()
        {
            Reduce();
            return TopologicalSort.For(this);
        }

        private static void MergeDependencies(IEnumerable<Dependency> set)
        {
            // merge all dep into first, adding predecessors and followers
            // remove deps from graph and from their predecessor and followers
            set.Aggregate((first, second) => first.Merge(second));
        }

        private static DependencyGraph CreateSimplifiedGraph(IEnumerable<Dependency> dependencies)
        {
            var simplifiedGraph = new DependencyGraph();
            var originalDependencies = dependencies.ToList();
            var newDependencies = originalDependencies.Select(d => simplifiedGraph.GetOrAdd(d.Type)).ToList();

            for (var i = 0; i < originalDependencies.Count; i++)
            {
                for (var j = i + 1; j < originalDependencies.Count; j++)
                {
                    if (originalDependencies[i].HasPredecessor(originalDependencies[j]))
                    {
                        newDependencies[i].After(newDependencies[j]);
                    }
                    else if (originalDependencies[i].HasFollower(originalDependencies[j]))
                    {
                        newDependencies[i].Before(newDependencies[j]);
                    }
                }
            }

            return simplifiedGraph;
        }

        internal static void CheckGraph(Dependency a, Dependency b)
        {
            if (a.Graph != b.Graph)
            {
                throw new ArgumentException($"dependency {a} is not associated with the same graph as dependency {b}");
            }
        }

        public void Remove(Dependency dependency)
        {
            _dependencies.Remove(dependency.Type);
        }
    }
}
