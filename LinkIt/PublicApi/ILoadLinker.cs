#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Responsible for loading and linking root linked sources
    /// </summary>
    public interface ILoadLinker<TRootLinkedSource>
    {
        Task<TRootLinkedSource> FromModelAsync<TRootLinkedSourceModel>(
            TRootLinkedSourceModel model,
            Action<TRootLinkedSource> initRootLinkedSource = null
        );

        Task<IReadOnlyList<TRootLinkedSource>> FromModelsAsync<TRootLinkedSourceModel>(
            IEnumerable<TRootLinkedSourceModel> models,
            Action<int, TRootLinkedSource> initRootLinkedSources = null
        );

        Task<TRootLinkedSource> ByIdAsync<TRootLinkedSourceModelId>(
            TRootLinkedSourceModelId modelId,
            Action<TRootLinkedSource> initRootLinkedSource = null
        );

        Task<IReadOnlyList<TRootLinkedSource>> ByIdsAsync<TRootLinkedSourceModelId>(
            IEnumerable<TRootLinkedSourceModelId> modelIds,
            Action<int, TRootLinkedSource> initRootLinkedSources = null
        );
    }
}