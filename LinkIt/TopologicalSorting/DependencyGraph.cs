// Copyright (c) 2016 Martin Evans
// Source: https://github.com/martindevans/TopologicalSorting
// Modified by Radio-Canada for the purposes of this library.

using System;
using System.Collections.Generic;

namespace LinkIt.TopologicalSorting
{
    /// <summary>
    /// A graph of object dependencies from which a topological sort can be extracted
    /// </summary>
    internal class DependencyGraph
    {
        private readonly Dictionary<Type, Dependency> _dependencies = new Dictionary<Type, Dependency>();

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

        public Dependency GetOrAdd(Type type)
        {
            if (_dependencies.TryGetValue(type, out var dependency))
            {
                return dependency;
            }

            dependency = new Dependency(this, type);

            return dependency;
        }

        public Dependency GetOrAdd(Type linkedSourceModelType, Type linkedSourceType)
        {
            if (_dependencies.TryGetValue(linkedSourceType, out Dependency dependency))
            {
                return dependency;
            }

            dependency = new Dependency(this, linkedSourceModelType, linkedSourceType);

            return dependency;
        }

        public void Add(Dependency dependency)
        {
            var type = dependency.LinkedSourceType ?? dependency.Type;
            if (!_dependencies.ContainsKey(type))
            {
                _dependencies.Add(type, dependency);
            }
        }

        internal static void CheckGraph(Dependency a, Dependency b)
        {
            if (a.Graph != b.Graph)
            {
                throw new ArgumentException($"dependency {a} is not associated with the same graph as dependency {b}");
            }
        }
    }
}
