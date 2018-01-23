#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;

namespace LinkIt.PublicApi
{
    /// <summary>
    /// Responsible for loading references of a loading level.
    /// </summary>
    /// <remarks>
    /// Dispose will always be invoked as soon as the load phase is completed or
    /// if an exception is thrown
    /// </remarks>
    public interface IReferenceLoader : IDisposable
    {
        Task LoadReferencesAsync(
            ILookupIdContext lookupIdContext,
            ILoadedReferenceContext loadedReferenceContext
        );
    }
}