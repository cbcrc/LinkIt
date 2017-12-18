#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.ReferenceTrees;

namespace LinkIt.Core.Includes
{
    //For a reference loaded by id (configured for specific discriminant of a link target):
    //responsible for loading and linking the link target for a specific discriminant
    //responsible for creating the reference tree for a specific discriminant
    public class IncludeReferenceById<TIReference, TLink, TReference, TId> :
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

        public void AddLookupId(TLink link, LookupIdContext lookupIdContext)
        {
            var lookupId = _getLookupId(link);
            lookupIdContext.AddSingle<TReference, TId>(lookupId);
        }

        public void AddReferenceTree(string linkTargetId, ReferenceTree parent, LoadLinkProtocol loadLinkProtocol)
        {
            new ReferenceTree(
                ReferenceType,
                linkTargetId,
                parent
            );
        }

        public TIReference GetReference(TLink link, LoadedReferenceContext loadedReferenceContext)
        {
            var lookupId = _getLookupId(link);
            return loadedReferenceContext.GetOptionalReference<TReference, TId>(lookupId);
        }
    }
}