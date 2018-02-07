// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.TopologicalSorting;

namespace LinkIt.Core.Includes
{
    /// <summary>
    /// For a reference loaded by id (configured for specific discriminant of a link target):
    /// responsible for loading and linking the link target for a specific discriminant
    /// responsible for creating the reference tree for a specific discriminant
    /// </summary>
    internal class IncludeReferenceById<TIReference, TLink, TReference, TId> :
        IIncludeWithGetReference<TIReference, TLink>,
        IIncludeWithAddLookupId<TLink>
        where TReference : TIReference
    {
        private readonly Func<TLink, TId> _getLookupId;

        public IncludeReferenceById(Func<TLink, TId> getLookupId)
        {
            _getLookupId = getLookupId;
            ReferenceType = typeof(TReference);
        }

        public Type ReferenceType { get; }

        public void AddLookupId(TLink link, LoadingContext loadingContext)
        {
            var lookupId = _getLookupId(link);
            loadingContext.AddLookupId<TReference>(lookupId);
        }

        public void AddDependency(string linkTargetId, Dependency predecessor, LoadLinkProtocol loadLinkProtocol)
        {
            var dependency = predecessor.Graph.GetOrAdd(ReferenceType);
            predecessor.Before(dependency);
        }

        public TIReference GetReference(TLink link, Linker linker)
        {
            var lookupId = _getLookupId(link);
            return linker.GetOptionalReference<TReference, TId>(lookupId);
        }
    }
}