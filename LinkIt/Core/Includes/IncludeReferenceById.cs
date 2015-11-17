using System;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.ReferenceTrees;

namespace LinkIt.Core.Includes
{
    public class IncludeReferenceById<TIReference, TLink, TReference, TId>: 
        IIncludeWithGetReference<TIReference, TLink>, 
        IIncludeWithAddLookupId<TLink>
        where TReference: TIReference
    {
        private readonly Func<TLink, TId> _getLookupId;

        public IncludeReferenceById(Func<TLink, TId> getLookupId){
            _getLookupId = getLookupId;
            ReferenceType = typeof(TReference);
        }

        public Type ReferenceType { get; private set; }

        public void AddLookupId(TLink link, LookupIdContext lookupIdContext)
        {
            var lookupId = _getLookupId(link);
            lookupIdContext.AddSingle<TReference, TId>(lookupId);
        }

        public TIReference GetReference(TLink link, LoadedReferenceContext loadedReferenceContext) {
            var lookupId = _getLookupId(link);
            return loadedReferenceContext.GetOptionalReference<TReference, TId>(lookupId);
        }

        public void AddReferenceTree(string linkTargetId, ReferenceTree parent, LoadLinkProtocol loadLinkProtocol){
            new ReferenceTree(
                ReferenceType, 
                linkTargetId, 
                parent
            );
        }
    }
}