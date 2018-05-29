// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.PublicApi;
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
        private readonly Dictionary<Type, List<ILoadLinkExpression>> _allLoadLinkExpressions;
        private readonly Func<IReferenceLoader> _createReferenceLoader;

        internal LoadLinkProtocol(
            IEnumerable<ILoadLinkExpression> loadLinkExpressions,
            Func<IReferenceLoader> createReferenceLoader)
        {
            _allLoadLinkExpressions = loadLinkExpressions
                .GroupBy(e => e.LinkedSourceType)
                .ToDictionary(g => g.Key, g => g.ToList());

            _createReferenceLoader = createReferenceLoader;
            InitLoadingLevelsForEachPossibleRootLinkedSourceType();
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

        internal List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource, Type referenceType)
        {
            return GetLoadLinkExpressions(linkedSource)
                .Where(loadLinkExpression => loadLinkExpression.ReferenceTypes.Contains(referenceType))
                .ToList();
        }

        internal List<ILoadLinkExpression> GetLoadLinkExpressions(object linkedSource)
        {
            return GetLoadLinkExpressions(linkedSource.GetType());
        }

        private List<ILoadLinkExpression> GetLoadLinkExpressions(Type linkedSourceType)
        {
            return _allLoadLinkExpressions.ContainsKey(linkedSourceType)
                ? _allLoadLinkExpressions[linkedSourceType]
                : new List<ILoadLinkExpression>();
        }

        private Dictionary<Type, List<List<Type>>> _loadingLevelsByRootLinkedSourceType;

        private List<List<Type>> GetLoadingLevelsFor<TRootLinkedSource>()
        {
            var rootLinkedSourceType = typeof(TRootLinkedSource);
            if (!_loadingLevelsByRootLinkedSourceType.ContainsKey(rootLinkedSourceType))
            {
                throw new InvalidOperationException(
                   $"The type {rootLinkedSourceType} cannot be used as root linked source because there are no load link expression associated with this linked source."
               );
            }

            return _loadingLevelsByRootLinkedSourceType[rootLinkedSourceType];
        }

        private void InitLoadingLevelsForEachPossibleRootLinkedSourceType()
        {
            _loadingLevelsByRootLinkedSourceType = new Dictionary<Type, List<List<Type>>>();
            foreach (var rootLinkedSourceType in _allLoadLinkExpressions.Keys)
            {
                var dependencyGraph = CreateDependencyGraph(rootLinkedSourceType);
                var sort = dependencyGraph.Sort();
                if (sort == null)
                {
                    throw new InvalidOperationException($"Cannot create the load link protocol for {rootLinkedSourceType}.");
                }

                var loadingLevels = sort.GetLoadingLevels();
                _loadingLevelsByRootLinkedSourceType.Add(rootLinkedSourceType, loadingLevels);
            }
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
                throw new NotSupportedException(
                    $"Unable to create the load link protocol for {rootLinkedSourceConfig.LinkedSourceType}. For more details, see inner exception.",
                    ex
                );
            }

            return dependencyGraph;
        }
    }
}