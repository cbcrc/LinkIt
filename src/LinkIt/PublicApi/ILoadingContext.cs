// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using LinkIt.Diagnostics;

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Responsible for giving access to the lookup ids of a loading level
    /// and for storing the loaded references.
    /// </summary>
    public interface ILoadingContext
    {
        /// <summary>
        /// Reference types to be loaded.
        /// </summary>
        IReadOnlyList<Type> ReferenceTypes { get; }

        /// <summary>
        /// Reference IDs to load for each type.
        /// </summary>
        IReadOnlyDictionary<Type, IReadOnlyList<object>> ReferenceIds();

        /// <summary>
        /// Get the IDs of the references to be loaded.
        /// </summary>
        /// <typeparam name="TReference">Type of reference</typeparam>
        /// <typeparam name="TId">Type of the ID</typeparam>
        IReadOnlyList<TId> ReferenceIds<TReference, TId>();

        /// <summary>
        /// Get the IDs of the references to be loaded.
        /// </summary>
        /// <param name="referenceType">Type of reference</param>
        /// <typeparam name="TId">Type of the ID</typeparam>
        IReadOnlyList<TId> ReferenceIds<TId>(Type referenceType);

        /// <summary>
        /// Add loaded references
        /// </summary>
        /// <typeparam name="TReference">Type of reference</typeparam>
        /// <typeparam name="TId">Type of the ID</typeparam>
        /// <param name="references">Enumerable references</param>
        /// <param name="getReferenceId">Function to get the ID from a reference</param>
        void AddResults<TReference, TId>(IEnumerable<TReference> references, Func<TReference, TId> getReferenceId);

        /// <summary>
        /// Add loaded references
        /// </summary>
        /// <typeparam name="TReference">Type of reference</typeparam>
        /// <typeparam name="TId">Type of the ID</typeparam>
        /// <param name="referencesById">Dictionary of references with their ID as key</param>
        void AddResults<TReference, TId>(IDictionary<TId, TReference> referencesById);

        /// <summary>
        /// Details about the current load-link operation.
        /// Debug mode must be enabled on the ILoadLinkProtocol for this to not be null.
        /// </summary>
        ILoadLinkDetails LoadLinkDetails { get; }
    }
}
