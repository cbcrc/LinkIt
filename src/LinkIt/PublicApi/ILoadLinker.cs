// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkIt.Diagnostics;

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Responsible for loading and linking root linked sources
    /// </summary>
    public interface ILoadLinker<TRootLinkedSource>
    {
        /// <summary>
        /// Load and link a linked source using a root model.
        /// </summary>
        Task<TRootLinkedSource> FromModelAsync<TRootLinkedSourceModel>(
            TRootLinkedSourceModel model,
            Action<TRootLinkedSource> initRootLinkedSource = null
        );

        /// <summary>
        /// Load and link a list of linked sources using root models.
        /// </summary>
        Task<IReadOnlyList<TRootLinkedSource>> FromModelsAsync<TRootLinkedSourceModel>(
            IEnumerable<TRootLinkedSourceModel> models,
            Action<int, TRootLinkedSource> initRootLinkedSources = null
        );

        /// <summary>
        /// Load and link a linked source using the ID of the root model.
        /// </summary>
        Task<TRootLinkedSource> ByIdAsync<TRootLinkedSourceModelId>(
            TRootLinkedSourceModelId modelId,
            Action<TRootLinkedSource> initRootLinkedSource = null
        );

        /// <summary>
        /// Load and link a list of linked sources using the IDs of the root models.
        /// </summary>
        Task<IReadOnlyList<TRootLinkedSource>> ByIdsAsync<TRootLinkedSourceModelId>(
            IEnumerable<TRootLinkedSourceModelId> modelIds,
            Action<int, TRootLinkedSource> initRootLinkedSources = null
        );

        /// <summary>
        /// Enable debug mode to provide information to aid in debugging load-link operations.
        /// An object of type <see cref="ILoadLinkDetails"/> will be available in the <see cref="ILoadingContext"/> object
        /// when method <see cref="IReferenceLoader.LoadReferencesAsync"/> is called.
        /// Also, an action can be supplied and will be called, with the <see cref="ILoadLinkDetails"/> object,
        /// when the load link operation is completed.
        /// </summary>
        /// <param name="onLoadLinkCompleted">Action to execute when the load link operation is completed.</param>
        ILoadLinker<TRootLinkedSource> EnableDebugMode(Action<ILoadLinkDetails> onLoadLinkCompleted = null);
    }
}
