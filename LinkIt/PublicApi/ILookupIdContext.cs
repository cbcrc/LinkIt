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
    /// Responsible for giving access to the lookup ids of a loading level.
    /// </summary>
    public interface ILookupIdContext
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
    }
}