// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Represents stats for a <see cref="ILoadLinkProtocol"/>.
    /// </summary>
    public class LoadLinkProtocolStatistics
    {
        private readonly Dictionary<Type, List<List<Type>>> _loadingLevelsByRootLinkedSourceType;

        internal LoadLinkProtocolStatistics(Dictionary<Type, List<List<Type>>> loadingLevelsByRootLinkedSourceType)
        {
            _loadingLevelsByRootLinkedSourceType = loadingLevelsByRootLinkedSourceType;
        }

        /// <summary>
        /// Number of root linked source types configured.
        /// </summary>
        public int NumberOfLinkedSources => _loadingLevelsByRootLinkedSourceType.Count;

        /// <summary>
        /// Number of loading steps for each linked source type.
        /// </summary>
        public Dictionary<Type, int> LoadingLevelDepthForEachLinkedSource
            => _loadingLevelsByRootLinkedSourceType.ToDictionary(
                item => item.Key,
                item => item.Value.Count);

        /// <summary>
        /// Number of reference types to be loaded for each linked source type.
        /// </summary>
        public Dictionary<Type, int> NumberOfReferenceTypeForEachLinkedSource
            => _loadingLevelsByRootLinkedSourceType.ToDictionary(
                    item => item.Key,
                    item => item.Value.Sum(referencesForOneLoadingLevel => referencesForOneLoadingLevel.Count));

        /// <summary>
        /// List of loading levels for the linked source types.
        /// </summary>
        public Dictionary<Type, List<List<Type>>> LoadingLevelsForEachLinkedSource
          => _loadingLevelsByRootLinkedSourceType.ToDictionary(item => item.Key, CloneLoadingLevels);

        //Ensure statistics cannot have side effect on load link protocol
        private static List<List<Type>> CloneLoadingLevels(KeyValuePair<Type, List<List<Type>>> item)
        {
            return item.Value
                    .Select(CloneLoadingLevel)
                    .ToList();
        }

        private static List<Type> CloneLoadingLevel(List<Type> referenceTypes)
        {
            return referenceTypes.ToList();
        }
    }
}