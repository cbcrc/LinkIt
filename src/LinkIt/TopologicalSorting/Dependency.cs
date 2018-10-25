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
    internal class Dependency : IEquatable<Dependency>
    {
        /// <summary>
        /// The type of this dependency.
        /// </summary>
        public DependencyType Type { get; }

        /// <summary>
        /// The graph this dependency is part of
        /// </summary>
        private readonly DependencyGraph _graph;

        private readonly HashSet<Dependency> _directPredecessors = new HashSet<Dependency>();
        public IEnumerable<Dependency> DirectPredecessors => _directPredecessors;

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
        /// Indicates that this dependency should be loaded before another
        /// </summary>
        /// <param name="follower">The ancestor.</param>
        public void Before(Dependency follower)
        {
            if (_directFollowers.Contains(follower))
            {
                return;
            }

            if (this == follower || HasPredecessor(follower))
            {
                throw new InvalidOperationException($"Recursive dependency detected for {follower}.");
            }

            if (_directFollowers.Add(follower))
            {
                follower.After(this);
            }
        }

        public bool HasPredecessor(Dependency dependency)
        {
            return _directPredecessors.Contains(dependency) || _directPredecessors.Any(p => p.HasPredecessor(dependency));
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
        /// Indicates that this dependency must be loaded after the reference type and/or linked source.
        /// </summary>
        /// <param name="referenceType">The reference type that can only be loaded before this dependency.</param>
        /// <param name="linkedSourceType">The linked source type that can only be loaded before this dependency.</param>
        public Dependency After(Type referenceType, Type linkedSourceType = null)
        {
            var dependency = _graph.GetOrAdd(referenceType, linkedSourceType);
            After(dependency);
            return dependency;
        }

        /// <summary>
        /// Indicates that this dependency should be loaded after another
        /// </summary>
        /// <param name="predecessor">The predecessor.</param>
        public void After(Dependency predecessor)
        {
            if (_directPredecessors.Contains(predecessor))
            {
                return;
            }

            if (this == predecessor || HasFollower(predecessor))
            {
                throw new InvalidOperationException($"Recursive dependency detected for {predecessor}.");
            }

            if (_directPredecessors.Add(predecessor))
            {
                predecessor.Before(this);
            }
        }

        public bool HasFollower(Dependency dependency)
        {
            return _directFollowers.Contains(dependency) || _directFollowers.Any(p => p.HasFollower(dependency));
        }

        /// <summary>
        /// Indicates that this dependency must be loaded after all the predecessors
        /// </summary>
        /// <param name="predecessors">The predecessors.</param>
        public Dependency After(params Dependency[] predecessors)
        {
            return After(predecessors as IEnumerable<Dependency>);
        }

        /// <summary>
        /// Indicates that this dependency must be loaded after all the predecessors
        /// </summary>
        /// <param name="predecessors">The predecessors.</param>
        public Dependency After(IEnumerable<Dependency> predecessors)
        {
            foreach (var predecessor in predecessors)
            {
                After(predecessor);
            }

            return this;
        }

        public Dependency Merge(Dependency other)
        {
            foreach (var predecessor in other.DirectPredecessors)
            {
                After(predecessor);
            }

            foreach (var follower in other.DirectFollowers)
            {
                Before(follower);
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

        public bool Equals(Dependency other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Type.Equals(other.Type);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Dependency) obj);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
}
