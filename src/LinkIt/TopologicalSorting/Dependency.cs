// Copyright (c) 2016 Martin Evans. Licensed under the MIT license.
// Source: https://github.com/martindevans/TopologicalSorting
// Modified by Radio-Canada for the purposes of this library.

using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.TopologicalSorting
{
    /// <summary>
    /// A dependency in an object graph to load.
    /// </summary>
    internal class Dependency
    {
        /// <summary>
        /// The type of this dependency.
        /// </summary>
        public DependencyType Type { get; }

        /// <summary>
        /// The graph this dependency is part of
        /// </summary>
        private readonly DependencyGraph _graph;

        // DO NOT override Equals(), we want to use reference comparison
        private readonly HashSet<Dependency> _directPredecessors = new HashSet<Dependency>();
        public IEnumerable<Dependency> DirectPredecessors => _directPredecessors;

        // DO NOT override Equals(), we want to use reference comparison
        private readonly HashSet<Dependency> _directFollowers = new HashSet<Dependency>();
        public IEnumerable<Dependency> DirectFollowers => _directFollowers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dependency"/> class.
        /// </summary>
        /// <param name="graph">The graph which this dependency is part of</param>
        /// <param name="type">The type of this dependency</param>
        public Dependency(DependencyGraph graph, DependencyType type)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        /// Indicates that this dependency must be loaded before the reference type and/or linked source.
        /// </summary>
        /// <param name="referenceType">The reference type that can only be loaded after this dependency.</param>
        /// <param name="linkedSourceType">The linked source type that can only be loaded after this dependency.</param>
        /// <returns>The created dependency</returns>
        public Dependency Before(Type referenceType, Type linkedSourceType = null)
        {
            var dependency = _graph.GetOrAdd(referenceType, linkedSourceType);
            Before(dependency);
            return dependency;
        }

        /// <summary>
        /// Indicates that this dependency should be loaded before another
        /// </summary>
        /// <param name="follower">The ancestor.</param>
        public void Before(Dependency follower)
        {
            if (ReferenceEquals(this, follower))
            {
                throw CycleDetectedException(follower);
            }

            if (!AddFollower(follower))
            {
                return;
            }

            if (HasPredecessor(follower))
            {
                throw CycleDetectedException(follower);
            }

            follower.AddPredecessor(this);
        }

        private bool AddFollower(Dependency follower)
        {
            return _directFollowers.Add(follower);
        }

        private static InvalidOperationException CycleDetectedException(Dependency dependency)
        {
            return new InvalidOperationException($"Recursive dependency detected for {dependency}.");
        }

        public bool HasPredecessor(Dependency dependency)
        {
            return _directPredecessors.Contains(dependency) || _directPredecessors.Any(p => p.HasPredecessor(dependency));
        }

        /// <summary>
        /// Indicates that this dependency should be loaded after another
        /// </summary>
        /// <param name="predecessor">The predecessor.</param>
        public void After(Dependency predecessor)
        {
            if (ReferenceEquals(this, predecessor))
            {
                throw CycleDetectedException(predecessor);
            }

            if (!AddPredecessor(predecessor))
            {
                return;
            }

            if (HasFollower(predecessor))
            {
                throw CycleDetectedException(predecessor);
            }

            predecessor.AddFollower(this);
        }

        private bool AddPredecessor(Dependency predecessor)
        {
            return _directPredecessors.Add(predecessor);
        }

        public bool HasFollower(Dependency dependency)
        {
            return _directFollowers.Contains(dependency) || _directFollowers.Any(p => p.HasFollower(dependency));
        }

        public Dependency Merge(Dependency other)
        {
            foreach (var predecessor in other.DirectPredecessors)
            {
                if (AddPredecessor(predecessor))
                {
                    predecessor.AddFollower(this);
                }
            }

            foreach (var follower in other.DirectFollowers)
            {
                if (AddFollower(follower))
                {
                    follower.AddPredecessor(this);
                }
            }

            other.RemoveFromGraph();

            return this;
        }

        private void RemoveFromGraph()
        {
            foreach (var dependency in DirectPredecessors.Concat(DirectFollowers))
            {
                dependency.RemoveFromDependencies(this);
            }

            _graph.Remove(this);
        }

        private void RemoveFromDependencies(Dependency other)
        {
            _directPredecessors.Remove(other);
            _directFollowers.Remove(other);
        }

        /// <inheritdoc />
        public override string ToString() => Type.ToString();
    }
}
