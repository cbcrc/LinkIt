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
        internal LoadLinkProtocolStatistics(IReadOnlyDictionary<Type, IReadOnlyList<IReadOnlyList<Type>>> loadingLevelsByRootLinkedSourceType)
        {
            LoadingLevelsForEachLinkedSource = loadingLevelsByRootLinkedSourceType;
        }

        /// <summary>
        /// List of loading levels for the linked source types.
        /// </summary>
        public IReadOnlyDictionary<Type, IReadOnlyList<IReadOnlyList<Type>>> LoadingLevelsForEachLinkedSource { get; }

        /// <summary>
        /// Number of root linked source types configured.
        /// </summary>
        public int NumberOfLinkedSources => LoadingLevelsForEachLinkedSource.Count;

        /// <summary>
        /// Number of loading steps for each linked source type.
        /// </summary>
        public IReadOnlyDictionary<Type, int> LoadingLevelDepthForEachLinkedSource
            => LoadingLevelsForEachLinkedSource.ToDictionary(
                item => item.Key,
                item => item.Value.Count);

        /// <summary>
        /// Number of reference types to be loaded for each linked source type.
        /// </summary>
        public IReadOnlyDictionary<Type, int> NumberOfReferenceTypeForEachLinkedSource
            => LoadingLevelsForEachLinkedSource.ToDictionary(
                    item => item.Key,
                    item => item.Value.Sum(referencesForOneLoadingLevel => referencesForOneLoadingLevel.Count));
    }
}
