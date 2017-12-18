#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkIt.PublicApi
{
    //Responsible for loading and linking root linked sources
    public interface ILoadLinker<TRootLinkedSource>
    {
        Task<TRootLinkedSource> FromModelAsync<TRootLinkedSourceModel>(
            TRootLinkedSourceModel model,
            Action<TRootLinkedSource> initRootLinkedSource = null
        );

        Task<List<TRootLinkedSource>> FromModelsAsync<TRootLinkedSourceModel>(
            List<TRootLinkedSourceModel> models,
            Action<int, TRootLinkedSource> initRootLinkedSources = null
        );

        Task<TRootLinkedSource> ByIdAsync<TRootLinkedSourceModelId>(
            TRootLinkedSourceModelId modelId,
            Action<TRootLinkedSource> initRootLinkedSource = null
        );

        Task<List<TRootLinkedSource>> ByIdsAsync<TRootLinkedSourceModelId>(
            List<TRootLinkedSourceModelId> modelIds,
            Action<int, TRootLinkedSource> initRootLinkedSources = null
        );
    }
}