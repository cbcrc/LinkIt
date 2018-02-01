// Copyright (c) 2016 Martin Evans
// Source: https://github.com/martindevans/TopologicalSorting
// Modified by Radio-Canada for the purposes of this library.

using System.Collections.Generic;
using System.Linq;

namespace LinkIt.TopologicalSorting
{
    /// <summary>
    /// Represents a sorting solution
    /// </summary>
    internal class TopologicalSort : IEnumerable<ISet<Dependency>>, IEnumerable<Dependency>
    {
        private readonly IReadOnlyList<ISet<Dependency>> _collections;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopologicalSort"/> class.
        /// </summary>
        private TopologicalSort(IReadOnlyList<ISet<Dependency>> collections)
        {
            _collections = collections;
        }

        public static TopologicalSort For(DependencyGraph graph)
        {
            var levels = new List<ISet<Dependency>>();
            var unused = new HashSet<Dependency>(graph.Dependencies);
            while (unused.Count > 0)
            {
                // select dependencies which have no predecessors in the unused set,
                // which means that all their predecessors must either be used, or not exist, either way is fine
                var set = new HashSet<Dependency>(
                    unused.Where(p => !unused.Overlaps(p.DirectPredecessors))
                );

                if (set.Count == 0)
                {
                    return null;
                }

                unused.ExceptWith(set);
                levels.Add(set);
            }

            return new TopologicalSort(levels);
        }

        /// <summary>
        /// Gets the sets of dependencies in the order to be loaded. Dependencies in a set can be loaded in any order.
        /// </summary>
        public IReadOnlyList<ISet<Dependency>> DependencySets => _collections;

        /// <summary>
        /// Gets the dependencies in an order to be loaded.
        /// </summary>
        public IReadOnlyList<Dependency> Dependencies => _collections.SelectMany(set => set).ToList();

        #region IEnumerable

        /// <inheritdoc />
        /// <summary>
        /// Gets the enumerator which enumerates sets of dependencies, where a set can be loaded in any order.
        /// </summary>
        IEnumerator<ISet<Dependency>> IEnumerable<ISet<Dependency>>.GetEnumerator()
        {
            return _collections.GetEnumerator();
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns an enumerator that iterates through the dependencies.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return (this as IEnumerable<Dependency>).GetEnumerator();
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the enumerator which enumerates through the dependencies in an order to be executed.
        /// </summary>
        /// <returns></returns>
        IEnumerator<Dependency> IEnumerable<Dependency>.GetEnumerator()
        {
            IEnumerable<IEnumerable<Dependency>> collections = this;

            return collections.SelectMany(collection => collection).GetEnumerator();
        }

        #endregion
    }
}
