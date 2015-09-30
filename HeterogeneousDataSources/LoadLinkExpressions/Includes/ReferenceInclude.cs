using System;

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

        //stle: really, does not make sense for reference
        public Type ChildLinkedSourceType { get { return null; } }

        //stle: dry with linked source
        public void AddLookupId(TLink link, LookupIdContext lookupIdContext)
        {
            var lookupId = _getLookupIdFunc(link);
            lookupIdContext.AddSingle<TReference>(lookupId);
        }

        public TIReference GetReference(TLink link, LoadedReferenceContext loadedReferenceContext) {
            //stle: dry with other load link expressions
            var lookupId = _getLookupIdFunc(link);
            return loadedReferenceContext.GetOptionalReference<TReference, TId>(lookupId);
        }
    }
}