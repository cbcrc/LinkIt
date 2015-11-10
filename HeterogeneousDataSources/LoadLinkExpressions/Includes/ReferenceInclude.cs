using System;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.ReferenceTrees;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public class ReferenceInclude<TIReference, TLink, TReference, TId>: 
        IIncludeWithGetReference<TIReference, TLink>, 
        IIncludeWithAddLookupId<TLink>
        where TReference: TIReference
    {
        private readonly Func<TLink, TId> _getLookupIdFunc;

        public ReferenceInclude(Func<TLink, TId> getLookupIdFunc){
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