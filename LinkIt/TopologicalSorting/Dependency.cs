// Copyright (c) 2016 Martin Evans
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
        /// The object type of this dependency.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The linked sourc type of this dependency.
        /// </summary>
        public Type LinkedSourceType { get; }

        /// <summary>
        /// The graph this dependency is part of
        /// </summary>
        public DependencyGraph Graph { get; }

        private readonly HashSet<Dependency> _predecessors = new HashSet<Dependency>();
        /// <summary>
        /// Gets the predecessors of this dependency
        /// </summary>
        /// <value>The predecessors.</value>
        public IEnumerable<Dependency> Predecessors => _predecessors;

        private readonly HashSet<Dependency> _followers = new HashSet<Dependency>();
        /// <summary>
        /// Gets the followers of this dependency
        /// </summary>
        public IEnumerable<Dependency> Followers => _followers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dependency"/> class.
        /// </summary>
        /// <param name="graph">The graph which this dependency is part of</param>
        /// <param name="type">The type of this dependency</param>
        public Dependency(DependencyGraph graph, Type type)
        {
            Graph = graph;
            Type = type;

            Graph.Add(this);
        }

        public Dependency(DependencyGraph graph, Type linkedSourceModelType, Type linkedSourceType)
        {
            Graph = graph;
            Type = linkedSourceModelType;
            LinkedSourceType = linkedSourceType;

            Graph.Add(this);
        }

        /// <summary>
        /// Indicates that this dependency should be loaded before another
        /// </summary>
        /// <param name="follower">The ancestor.</param>
        /// <returns>returns this dependency</returns>
        public Dependency Before(Dependency follower)
        {
            DependencyGraph.CheckGraph(this, follower);

            if (this == follower || HasPredecessor(follower))
            {
                throw new InvalidOperationException($"Recursive dependency detected for {follower}.");
            }

            if (_followers.Add(follower))
            {
                follower.After(this);
            }

            return this;
        }

        public bool HasPredecessor(Dependency dependency)
        {
            return _predecessors.Contains(dependency) || _predecessors.Any(p => p.HasPredecessor(dependency));
        }

        /// <summary>
        /// Indicates that this dependency must be loaded before all the followers
        /// </summary>
        /// <param name="followers">The followers.</param>
        /// <returns>the followers</returns>
        public Dependency Before(params Dependency[] followers)
        {
            return Before(followers as IEnumerable<Dependency>);
        }

        /// <summary>
        /// Indicates that this dependency must be loaded before all the followers
        /// </summary>
        /// <param name="followers">The followers.</param>
        public Dependency Before(IEnumerable<Dependency> followers)
        {
            foreach (var ancestor in followers)
            {
                Before(ancestor);
            }

            return this;
        }

        /// <summary>
        /// Indicates that this dependency should be loaded after another
        /// </summary>
        /// <param name="predecessor">The predecessor.</param>
        /// <returns>this dependency</returns>
        public Dependency After(Dependency predecessor)
        {
            DependencyGraph.CheckGraph(this, predecessor);

            if (this == predecessor || HasFollower(predecessor))
            {
                throw new InvalidOperationException($"Recursive dependency detected for {predecessor}.");
            }

            if (_predecessors.Add(predecessor))
            {
                predecessor.Before(this);
            }

            return this;
        }

        public bool HasFollower(Dependency dependency)
        {
            return _followers.Contains(dependency) || _followers.Any(p => p.HasFollower(dependency));
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

        /// <inheritdoc />
        public override string ToString() => $"type {{ {LinkedSourceType?.Name ?? Type.Name} }}";
    }
}
