// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkIt.PublicApi
{
    /// <summary>
    ///     Simple data loader for reference types.
    /// </summary>
    public interface IDataLoader<TModel>
    {
        /// <summary>
        ///     Load an entity by ID.
        /// </summary>
        Task<TModel> ByIdAsync<TId>(TId id);

        /// <summary>
        ///     Load entities by their IDs.
        /// </summary>
        Task<IReadOnlyList<TModel>> ByIdsAsync<TId>(IEnumerable<TId> ids);
    }
}
