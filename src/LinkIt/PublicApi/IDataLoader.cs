using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Simple data loader for reference types.
    /// </summary>
    public interface IDataLoader<TModel>
    {
        /// <summary>
        /// Load an entity by ID.
        /// </summary>
        Task<TModel> ByIdAsync<TId>(TId id);

        /// <summary>
        /// Load entities by their IDs.
        /// </summary>
        Task<IReadOnlyList<TModel>> ByIdsAsync<TId>(IEnumerable<TId> ids);
    }
}