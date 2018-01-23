#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Responsible for gathering all loaded references of a root linked source.
    /// </summary>
    public interface ILoadedReferenceContext
    {
        void AddReferences<TReference, TId>(IEnumerable<TReference> references, Func<TReference, TId> getReferenceId);
        void AddReferences<TReference, TId>(IDictionary<TId, TReference> referencesById);
    }
}