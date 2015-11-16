using System;
using LinkIt.LoadLinkExpressions.Includes.Interfaces;
using LinkIt.Protocols;
using LinkIt.ReferenceTrees;

namespace LinkIt.LoadLinkExpressions.Includes
{
    public class IncludeReferenceById<TIReference, TLink, TReference, TId>: 
        IIncludeWithGetReference<TIReference, TLink>, 
        IIncludeWithAddLookupId<TLink>
        where TReference: TIReference
    {
        private readonly Func<TLink, TId> _getLookupIdFunc;

        public IncludeReferenceById(Func<TLink, TId> getLookupIdFunc){
            _getLookupIdFunc = getLookupIdFunc;
            ReferenceType = typeof(TReference);
        }

        public Type ReferenceType { get; private set; }

        public void AddLookupId(TLink link, LookupIdContext lookupIdContext)
        {
            var lookupId = _getLookupIdFunc(link);
            lookupIdContext.AddSingle<TReference, TId>(lookupId);
        }

        public TIReference GetReference(TLink link, LoadedReferenceContext loadedReferenceContext) {
            var lookupId = _getLookupIdFunc(link);
            return loadedReferenceContext.GetOptionalReference<TReference, TId>(lookupId);
        }

        public void AddReferenceTree(string linkTargetId, ReferenceTree parent, LoadLinkConfig config){
            new ReferenceTree(
                ReferenceType, 
                linkTargetId, 
                parent
            );
        }
    }
}