#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Responsible for giving access to the lookup ids of a loading level
    /// and for storing the loaded references.
    /// </summary>
    public interface ILoadingContext
    {
        /// <summary>
        /// Get reference types to be loaded.
        /// </summary>
        IReadOnlyList<Type> GetReferenceTypes();

        /// <summary>
        /// Get reference types with their IDs to be loaded.
        /// </summary>
        IDictionary<Type, IEnumerable> GetReferenceIds();

        /// <summary>
        /// Get the IDs of the references to be loaded.
        /// </summary>
        /// <typeparam name="TReference">Type of reference</typeparam>
        /// <typeparam name="TId">Type of the ID</typeparam>
        IReadOnlyList<TId> GetReferenceIds<TReference, TId>();

        /// <summary>
        /// Get the IDs of the references to be loaded.
        /// </summary>
        /// <param name="referenceType">Type of reference</param>
        /// <typeparam name="TId">Type of the ID</typeparam>
        IReadOnlyList<TId> GetReferenceIds<TId>(Type referenceType);

        /// <summary>
        /// Add loaded references
        /// </summary>
        /// <typeparam name="TReference">Type of reference</typeparam>
        /// <typeparam name="TId">Type of the ID</typeparam>
        /// <param name="references">Enumerable references</param>
        /// <param name="getReferenceId">Function to get the ID from a reference</param>
        void AddReferences<TReference, TId>(IEnumerable<TReference> references, Func<TReference, TId> getReferenceId);

        /// <summary>
        /// Add loaded references
        /// </summary>
        /// <typeparam name="TReference">Type of reference</typeparam>
        /// <typeparam name="TId">Type of the ID</typeparam>
        /// <param name="referencesById">Dictionary of references with their ID as key</param>
        void AddReferences<TReference, TId>(IDictionary<TId, TReference> referencesById);
    }
}