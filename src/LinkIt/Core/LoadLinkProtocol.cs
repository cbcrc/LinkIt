// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LinkIt.PublicApi;
using LinkIt.ReadableExpressions.Extensions;
using LinkIt.Shared;
using LinkIt.TopologicalSorting;

namespace LinkIt.Core
{
    /// <summary>
    /// In addition to the responsiblies of ILoadLinkProtocol,
    /// responsible for gathering and giving access to the load link expressions
    /// responsible to infer loading levels for each possible root linked source
    /// </summary>
    internal class LoadLinkProtocol : ILoadLinkProtocol
    {
        private readonly IReadOnlyDictionary<Type, ImmutableList<ILoadLinkExpression>> _allLoadLinkExpressions;
        private readonly Func<IReferenceLoader> _createReferenceLoader;
        private readonly IReadOnlyDictionary<Type, IReadOnlyList<IReadOnlyList<Type>>> _loadingLevelsByRootLinkedSourceType;

        internal LoadLinkProtocol(
            IEnumerable<ILoadLinkExpression> loadLinkExpressions,
            Func<IReferenceLoader> createReferenceLoader)
        {
            _createReferenceLoader = createReferenceLoader;

            _allLoadLinkExpressions = loadLinkExpressions
                .GroupBy(e => e.LinkedSourceType)
                .ToImmutableDictionary(g => g.Key, g => g.ToImmutableList());

            _loadingLevelsByRootLinkedSourceType = _allLoadLinkExpressions.Keys
                .ToImmutableDictionary(linkedSourceType => linkedSourceType, CalculateLoadingLevels);
        }

        public ILoadLinker<TRootLinkedSource> LoadLink<TRootLinkedSource>()
        {
            return LinkedSourceConfigs.GetConfigFor<TRootLinkedSource>().CreateLoadLinker(
                _createReferenceLoader,
                GetLoadingLevelsFor<TRootLinkedSource>(),
                this
            );
        }

        public IDataLoader<TModel> Load<TModel>()
        {
            return new DataLoader<TModel>(_createReferenceLoader);
        }

        public LoadLinkProtocolStatistics Statistics => new LoadLinkProtocolStatistics(_loadingLevelsByRootLinkedSourceType);

        internal bool IsDebugModeEnabled { get; private set; }

        public void EnableDebugMode()
        {
            IsDebugModeEnabled = true;
        }

        internal IReadOnlyList<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, Type referenceType)
        {
            return GetLoadLinkExpressions(linkedSource)
                .Where(loadLinkExpression => loadLinkExpression.ReferenceTypes.Contains(referenceType))
                .ToList();
        }

        internal IReadOnlyList<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource)
        {
            return GetLoadLinkExpressions(linkedSource.GetType());
        }

        private IReadOnlyList<ILoadLinkExpression> GetLoadLinkExpressions(Type linkedSourceType)
        {
            return _allLoadLinkExpressions.ContainsKey(linkedSourceType)
                ? (IReadOnlyList<ILoadLinkExpression>) _allLoadLinkExpressions[linkedSourceType]
                : new List<ILoadLinkExpression>();
        }

        private IReadOnlyList<IReadOnlyList<Type>> GetLoadingLevelsFor<TRootLinkedSource>()
        {
            var rootLinkedSourceType = typeof(TRootLinkedSource);
            if (!_loadingLevelsByRootLinkedSourceType.ContainsKey(rootLinkedSourceType))
            {
                throw new LinkItException(
                   $"Cannot find load link configuration for type {rootLinkedSourceType.GetFriendlyName()}."
               );
            }

            return _loadingLevelsByRootLinkedSourceType[rootLinkedSourceType];
        }

        private IReadOnlyList<IReadOnlyList<Type>> CalculateLoadingLevels(Type rootLinkedSourceType)
        {
            var dependencyGraph = CreateDependencyGraph(rootLinkedSourceType);
            var sort = dependencyGraph.Sort();
            if (sort == null)
            {
                throw new LinkItException($"Cannot create load link protocol for {rootLinkedSourceType.GetFriendlyName()}. Possible cyclic dependencies.");
            }

            return sort.GetLoadingLevels();
        }

        internal void AddDependenciesForAllLinkTargets(Type linkedSourceType, Dependency predecessor)
        {
            foreach (var loadLinkExpression in GetLoadLinkExpressions(linkedSourceType))
            {
                loadLinkExpression.AddDependencyForEachInclude(predecessor, this);
            }
        }

        internal DependencyGraph CreateDependencyGraph(Type rootLinkedSourceType)
        {
            var dependencyGraph = new DependencyGraph();

            var rootLinkedSourceConfig = LinkedSourceConfigs.GetConfigFor(rootLinkedSourceType);
            var rootDependency = dependencyGraph.GetOrAdd(rootLinkedSourceConfig.LinkedSourceModelType, rootLinkedSourceConfig.LinkedSourceType);

            try
            {
                AddDependenciesForAllLinkTargets(rootLinkedSourceConfig.LinkedSourceType, rootDependency);
            }
            catch (NotSupportedException ex)
            {
                throw new LinkItException(
                    $"Unable to create the load link protocol for {rootLinkedSourceType.GetFriendlyName()}. For more details, see inner exception.",
                    ex
                );
            }

            return dependencyGraph;
        }
    }
}
